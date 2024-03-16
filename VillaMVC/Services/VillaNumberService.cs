using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;

namespace VillaMVC.Service;

public class VillaNumberService : IVillaNumberService
{
    private readonly IConfiguration _configuration;
    private readonly IBaseService _baseService;
    public string Domain { get; set; }
    public VillaNumberService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _configuration = configuration;
        _baseService = baseService;
        Domain = _configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T> Get<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Get,
            URL = Domain + $"/api/VillaNumber/{id}"
        });
    }

    public async Task<T> GetAll<T>()
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Get,
            URL = Domain + $"/api/VillaNumber"
        });
    }

    public async Task<T> Create<T>(VillaNumberDto villaNumberDto)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Post,
            URL = Domain + $"/api/VillaNumber",
            Data = villaNumberDto
        });
    }

    public async Task<T> Update<T>(int id, VillaNumberDto villaNumberDto)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Put,
            URL = Domain + $"/api/VillaNumber/{id}",
            Data = villaNumberDto
        });
    }

    public async Task<T> Delete<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Delete,
            URL = Domain + $"/api/VillaNumber/{id}"
        });
    }
}