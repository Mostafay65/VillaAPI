using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using NuGet.Protocol;
using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Controllers;

public class UserController : Controller
{
    private readonly ILocalUserService _localUserService;
    private readonly ITokenProvider _tokenProvider;

    public UserController(ILocalUserService localUserService, ITokenProvider tokenProvider)
    {
        _localUserService = localUserService;
        _tokenProvider = tokenProvider;
    }
    // GET
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDTO loginRequestDto)
    {
        APIResponse response = await _localUserService.Login<APIResponse>(loginRequestDto);
        if (!response.Success)
        {
            ModelState.AddModelError("", response.ErrorMessages[0]);
            return View(loginRequestDto);
        }

        LoginResponseDTO loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDTO>(response.Result.ToString());
        _tokenProvider.setToken(loginResponseDto.Token);
        // HttpContext.Session.SetString(SD.SessionToken,loginResponseDto.Token);
        // HttpContext.Session.SetString("Name",loginResponseDto.User.Name);

        // var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        // identity.AddClaim(new Claim(ClaimTypes.Name, loginResponseDto.User.UserName));
        // identity.AddClaim(new Claim(ClaimTypes.Role, loginResponseDto.User.Role));
        // var principal = new ClaimsPrincipal(identity);
        // await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        
        
        
        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, loginResponseDto.User.UserName));
        identity.AddClaim(new Claim(ClaimTypes.Role, loginResponseDto.Role));
        // var jwtSecurityToken = new JwtSecurityTokenHandler();
        // var jwt = jwtSecurityToken.ReadJwtToken(loginResponseDto.Token);
        // identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(c=>c.Type == "role").Value));
        // identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(c=>c.Type == "name").Value));

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        
        return RedirectToAction("Index","Home");
    }
    // GET
    public IActionResult Register()
    {
        List<SelectListItem> Roles = new List<SelectListItem>()
        {
            new SelectListItem() { Text = SD.Admin, Value = SD.Admin },
            new SelectListItem() { Text = SD.Custrom, Value = SD.Custrom }
        };
        ViewBag.Roles = Roles;
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterRequestDTO registerRequest)
    {
        List<SelectListItem> Roles = new List<SelectListItem>()
        {
            new SelectListItem() { Text = SD.Admin, Value = SD.Admin },
            new SelectListItem() { Text = SD.Custrom, Value = SD.Custrom }
        };
        ViewBag.Roles = Roles;
        if (!ModelState.IsValid)
        {
            return View(registerRequest);
        }
        APIResponse response = await _localUserService.Register<APIResponse>(registerRequest);
        if (response is null || !response.Success)
        {
            ModelState.AddModelError("", response.ErrorMessages[0]);
            return View(registerRequest);
        }

        UserDTO user = JsonConvert.DeserializeObject<UserDTO>(response.Result.ToString());
        return RedirectToAction("Index","Home");
    }

    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();
        _tokenProvider.ClearToken();
        // HttpContext.Session.SetString(SD.SessionToken, ""); 
        // HttpContext.Session.SetString("Name", ""); 
        return RedirectToAction("index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
    
}