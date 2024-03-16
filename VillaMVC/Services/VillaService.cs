using VillaMVC.Models;
using VillaMVC.Models.DTO;
using VillaMVC.Service.IService;
using VillaUtility;

namespace VillaMVC.Service;

public class VillaService : IVillaService
{
    private readonly IConfiguration _configuration;
    private readonly IBaseService _baseService;
    private string domain;

    public VillaService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IBaseService baseService)
    {
        _configuration = configuration;
        _baseService = baseService;
        domain = configuration["ServiceUrls:VillaAPI"];
    }

    public async Task<T> Get<T>(int id)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Get,
            Data = null,
            URL = domain + $"/api/Villa/{id}"
        });
    }
    

    public async Task<T> GetAll<T>()
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Get,
            Data = null,
            URL = domain + $"/api/Villa"
        });
    }

    public async Task<T> update<T>(int id, VillaDTO villa )
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Put,
            Data = villa,
            URL = domain + $"/api/Villa/{id}",
            ContentType = SD.ContentType.MultipartFormData
        });
    }

    public async Task<T> Create<T>(VillaDTO villa)
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Post,
            Data = villa,
            URL = domain + $"/api/Villa",
            ContentType = SD.ContentType.MultipartFormData
        });
    }

    public async Task<T> Delete<T>(int id )
    {
        return await _baseService.SendAsync<T>(new APIRequest()
        {
            Method = HttpMethod.Delete,
            Data = null,
            URL = domain + $"/api/Villa/{id}"
        });
    }
}