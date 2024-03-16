using Microsoft.AspNetCore.Identity;
using VillaAPI.Models.DTO;

namespace VillaMVC.Service.IService;

public interface ITokenProvider
{
    void setToken(TokenDTO token);
    TokenDTO GetToken();
    void ClearToken();
}