using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shire.WebMvc.Entities;
using System.Reflection;

namespace Shire.WebMvc.Contexts;

//public class AppDbContext : DbContext
public class AppDbContext : IdentityDbContext<User, UserRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}
    public DbSet<Category> Categories { get; set; }
    public DbSet<Basket> Basket { get; set; }
    public DbSet<Product> Product { get; set; }
    public DbSet<SubCategory> SubCategory { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    //}
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<IdentityUserLogin<string>>(b =>
        {
            b.HasKey(x => new { x.LoginProvider, x.ProviderKey });
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
    }



}
