using VillaMVC.Models.DTO;
using VillaMVC.Models;
using VillaMVC.Service.IService;

namespace VillaMVC.Service;

public class LocalUserService : ILocalUserService
{
    private readonly IConfiguration _configuration;
    private readonly IBaseService _baseService;
    private string Domain;
    public LocalUserService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _configuration = configuration;
        _baseService = baseService;
        Domain = _configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T> Login<T>(LoginRequestDTO loginRequestDto)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Post,
            Data = loginRequestDto,
            URL = Domain + "/api/User/Login"
        });
    }

    public async Task<T> Register<T>(RegisterRequestDTO registerRequest)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Post,
            Data = registerRequest,
            URL = Domain + "/api/User/Register"
        });
    }
}