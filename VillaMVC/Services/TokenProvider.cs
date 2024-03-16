using VillaAPI.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Service;

public class TokenProvider : ITokenProvider
{
    private readonly IHttpContextAccessor _accessor;

    public TokenProvider(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }
    public void setToken(TokenDTO token)
    {
        var options = new CookieOptions() { Expires = DateTimeOffset.Now.AddHours(30) };
        _accessor.HttpContext.Response.Cookies.Append(SD.AccessToken,token.AccessToken, options);
        _accessor.HttpContext.Response.Cookies.Append(SD.RefreshToken,token.RefreshToken, options);
    }

    public TokenDTO GetToken()
    {
        bool HasAccessToken = _accessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string Accesstoken);
        bool HasRefreshToken = _accessor.HttpContext.Request.Cookies.TryGetValue(SD.RefreshToken, out string Refreshtoken);
        return HasAccessToken? new TokenDTO() { AccessToken = Accesstoken, RefreshToken = Refreshtoken } : null;
    }

    public void ClearToken()
    {
        _accessor.HttpContext.Response.Cookies.Delete(SD.AccessToken);
        _accessor.HttpContext.Response.Cookies.Delete(SD.RefreshToken);
    }
}