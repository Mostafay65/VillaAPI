using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class VillaRepository :  Repository<Villa>, IVillaRepository 
{
    private readonly ApplicationDbContext _context;

    public VillaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task UpdateAsync(Villa villa)
    {
        _context.Update(villa);
        await SaveAsync();
    }
}
