using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Models.DTO;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class LocalUserRepository : ILocalUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public LocalUserRepository(ApplicationDbContext context , IMapper mapper,
        IConfiguration configuration, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<bool> Is_Unique(string UserName)
    {
        return await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == UserName) == null;
    }

    public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest)
    {
        var user = await _userManager.FindByNameAsync(loginRequest.UserName.ToLower());
        var isValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
        if (user is null || !isValid)
            return new LoginResponseDTO()
            {
                Token = new TokenDTO { AccessToken = "" },
                User = null
            };
        var roles = await _userManager.GetRolesAsync(user);
        var TokenId = $"JTI{Guid.NewGuid()}";
        var Token = await CreatToken(user, TokenId);
        var refreshToken = await CreatRefreshToken(user.Id, TokenId);
        LoginResponseDTO loginResponseDto = new()
        {
            Token = new TokenDTO
            {
                AccessToken = Token,
                RefreshToken = refreshToken
            },
            User = _mapper.Map<LocalUserDTO>(user),
            Role = roles.FirstOrDefault()
        };
        return loginResponseDto;
    }

    public async Task<LocalUserDTO> Register(RegisterRequestDTO RegisterRequest)
    {
        if (!await Is_Unique(RegisterRequest.UserName)) return null;

        var user = new ApplicationUser()
        {
            Name = RegisterRequest.Name,
            Email = RegisterRequest.UserName,
            NormalizedEmail = RegisterRequest.UserName.ToUpper(),
            NormalizedUserName = RegisterRequest.UserName.ToUpper(),
            UserName = RegisterRequest.UserName
        };
        try
        {
            var result = await _userManager.CreateAsync(user, RegisterRequest.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(RegisterRequest.Role))
                    await _roleManager.CreateAsync(new IdentityRole(RegisterRequest.Role));
                await _userManager.AddToRoleAsync(user, RegisterRequest.Role);
                return _mapper.Map<LocalUserDTO>(user);
            }
            else
            {
                return new LocalUserDTO()
                {
                    UserName = "Error",
                    Name = result.Errors.FirstOrDefault().Description
                };
            }
        }
        catch (Exception e)
        {
            return new LocalUserDTO()
            {
                Name = e.Message
            };
        }
    }

    private bool GetAccessTokenData(string accessToken, string expectedUserId, string expectedTokenId)
    {
        try
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var jwtTokenId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Jti).Value;
            var userId = jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value;
            return userId == expectedUserId && jwtTokenId == expectedTokenId;
        }
        catch
        {
            return false;
        }
    }

    public async Task<string> CreatToken(ApplicationUser user, string TokenId)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var SecretKey = _configuration["APISettings:SecretKey"];
        var TokenHandler = new JwtSecurityTokenHandler();
        var Key = Encoding.ASCII.GetBytes(SecretKey);
        var TokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(
                new Claim[]
                {
                    new(ClaimTypes.Name, user.Id),
                    new(ClaimTypes.Role, roles.FirstOrDefault()),
                    new(JwtRegisteredClaimNames.Jti, TokenId),
                    new(JwtRegisteredClaimNames.Sub, user.Id)
                }),
            Expires = DateTime.Now.AddMinutes(120),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = TokenHandler.CreateToken(TokenDescriptor);
        return TokenHandler.WriteToken(token);
    }

    public async Task<string> CreatRefreshToken(string userId, string TokenId)
    {
        var refreshToken = new RefreshToken()
        {
            IsValid = true,
            JwtTokenId = TokenId,
            UserId = userId,
            ExpireAt = DateTime.Now.AddDays(30),
            Refresh_Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
        return refreshToken.Refresh_Token;
    }

    public async Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDto)
    {
        // Find an existing refresh token
        var existingRefreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(u => u.Refresh_Token == tokenDto.RefreshToken);
        if (existingRefreshToken == null) return new TokenDTO();

        // Compare data from existing refresh and access token provided and if there is any missmatch then consider it as a fraud
        var isTokenValid = GetAccessTokenData(tokenDto.AccessToken, existingRefreshToken.UserId,
            existingRefreshToken.JwtTokenId);
        if (!isTokenValid)
        {
            await MarkTokenAsInvalid(existingRefreshToken);
            return new TokenDTO();
        }

        // When someone tries to use not valid refresh token, fraud possible
        if (!existingRefreshToken.IsValid)
            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        // If just expired then mark as invalid and return empty
        if (existingRefreshToken.ExpireAt < DateTime.Now)
        {
            await MarkTokenAsInvalid(existingRefreshToken);
            return new TokenDTO();
        }

        // replace old refresh with a new one with updated expire date
        var newRefreshToken = await CreatRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);


        // revoke existing refresh token
        await MarkTokenAsInvalid(existingRefreshToken);

        // generate new access token
        var applicationUser = _context.ApplicationUsers.FirstOrDefault(u => u.Id == existingRefreshToken.UserId);
        if (applicationUser == null)
            return new TokenDTO();

        var newAccessToken = await CreatToken(applicationUser, existingRefreshToken.JwtTokenId);

        return new TokenDTO()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
    {
        await _context.RefreshTokens.Where(u => u.UserId == userId
                                                && u.JwtTokenId == tokenId)
            .ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));
    }


    private Task MarkTokenAsInvalid(RefreshToken refreshToken)
    {
        refreshToken.IsValid = false;
        return _context.SaveChangesAsync();
    }
}