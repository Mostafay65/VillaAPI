using VillaAPI.Models.DTO;

namespace VillaAPI.Repository.IRepository;

public interface ILocalUserRepository
{
    public Task<bool> Is_Unique(string UserName);
    public Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
    public Task<LocalUserDTO> Register(RegisterRequestDTO RegisterRequest);
    public Task<TokenDTO> RefreshAccessToken(TokenDTO tokenDto);
}