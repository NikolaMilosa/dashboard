using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace dashboard.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Country> Countries { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IdentityRole>().HasData(new IdentityRole[] { new IdentityRole { Name = "admin", NormalizedName = "ADMIN" } });
        base.OnModelCreating(builder);
    }
}
