using VillaMVC.Models.DTO;

namespace VillaMVC.Service.IService;

public interface ILocalUserService
{
    public Task<T> Login<T>(LoginRequestDTO loginRequestDto);
    public Task<T> Register<T>(RegisterRequestDTO registerRequest);
}