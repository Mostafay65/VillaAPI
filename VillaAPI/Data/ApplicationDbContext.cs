using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Models;
using VillaAPI.Models.DTO;

namespace VillaAPI.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Villa> Villas { get; set; }
    public DbSet<VillaNumber> VillaNumbers { get; set; }
    public DbSet<LocalUser> LocalUsers { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Villa>().Property(v => v.ImageUrl).HasDefaultValue("https://dotnetmastery.com/bluevillaimages/villa2.jpg");
        builder.Entity<VillaNumber>().Property(v => v.Image).HasDefaultValue("https://dotnetmastery.com/bluevillaimages/villa2.jpg");
    }
}