using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using VillaAPI.Models;

namespace VillaAPI.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
    public Task UpdateAsync(Villa villa);
}