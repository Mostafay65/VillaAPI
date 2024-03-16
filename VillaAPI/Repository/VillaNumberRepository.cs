using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaNumberRepository : Repository<VillaNumber>, IVillaNumberRepository
{
    private readonly ApplicationDbContext _context;

    public VillaNumberRepository(ApplicationDbContext Context) : base(Context)
    {
        _context = Context;
    }
    
    public async Task UpdateAsync(VillaNumber villaNumber)
    {
        _context.VillaNumbers.Update(villaNumber);
        await SaveAsync();
        return;
    }
}