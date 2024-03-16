using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Data;
using VillaAPI.Models;
using VillaAPI.Repository.IRepository;

namespace VillaAPI.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> DbSet;

    public Repository(ApplicationDbContext Context)
    {
        _context = Context;
        DbSet = _context.Set<T>();
    }
    
    public async Task<T?> GetAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<T?> GetAsync(Expression<Func<T, bool>> Filter)
    {
        T T =  DbSet.FirstOrDefault(Filter);
        return T;
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> Filter = null, List<string> Includes = null)
    {
        IQueryable<T> Query = DbSet;
        if (Filter is not null)
        {
            Query = Query.Where(Filter);
        }

        if (Includes is not null)
        {
            foreach (string include in Includes)
            {
                Query = Query.Include(include);
            }
        }
        return await Query.ToListAsync();
    }

    public async Task AddAsync(T T)
    {
        await _context.AddAsync(T);
        await SaveAsync();
    }
    
    public async Task DeleteAsync(T T)
    {
        DbSet.Remove(T);
        await SaveAsync();
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}