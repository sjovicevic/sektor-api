using Microsoft.EntityFrameworkCore;
using Sektor.API.src.Configurations;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Context;

public class SektorContext : DbContext
{
    public DbSet<User> Users { get; set; } 
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MembershipType> MembershipTypes { get; set; }

    public SektorContext(DbContextOptions options) : base(options)
    {
        
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach(var entry in ChangeTracker.Entries())
        {
            if(entry.Entity is Entity e)
            {
                switch(entry.State)
                {
                    case EntityState.Added:
                        e.CreatedAt = DateTime.Now;
                        e.IsDeleted = false;
                        e.ModifiedAt = null;
                        e.DeletedAt = null;
                        e.IsActive = true;
                        break;
                    case EntityState.Modified:
                        e.ModifiedAt = DateTime.Now;
                        break;
                }
            }
        }
        return base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new MembershipConfiguration());

        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Membership>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<MembershipType>().HasQueryFilter(e => !e.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

}