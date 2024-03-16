using System.Linq.Expressions;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository;

public interface IRepository<T> where T : class
{
    public Task<T?> GetAsync(int id);
    public Task<T?> GetAsync(Expression<Func<T, bool>> Filter);
    public Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? Filter = null, List<string> Includes = null);
    public Task AddAsync(T villa);
    public Task DeleteAsync(T villa);
    public Task SaveAsync();
}