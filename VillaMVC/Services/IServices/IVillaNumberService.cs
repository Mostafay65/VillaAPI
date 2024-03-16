using VillaMVC.Models.DTO;

namespace VillaMVC.Service.IService;

public interface IVillaNumberService
{
    Task<T> Get<T>(int id);
    Task<T> GetAll<T>();
    Task<T> Create<T>(VillaNumberDto villaNumberDto);
    Task<T> Update<T>(int id, VillaNumberDto villaNumberDto);
    Task<T> Delete<T>(int id);
}