using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository;

public interface IVillaNumberRepository : IRepository<VillaNumber>
{
    public Task UpdateAsync(VillaNumber villaNumber);
}