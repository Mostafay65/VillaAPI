using VillaMVC.Models;

namespace VillaMVC.Service.IService;

public interface IBaseService
{
    public APIResponse _Response { get; set; }
    Task<T> SendAsync<T>(APIRequest apiRequest);
}