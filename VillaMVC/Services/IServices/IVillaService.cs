using VillaMVC.Models.DTO;

namespace VillaMVC.Service.IService;

public interface IVillaService
{
    public Task<T> Get<T>(int id);
    public Task<T> GetAll<T>();
    public Task<T> update<T>(int id, VillaDTO villa);
    public Task<T> Create<T>(VillaDTO villa);
    public Task<T> Delete<T>(int id);
    
    
}