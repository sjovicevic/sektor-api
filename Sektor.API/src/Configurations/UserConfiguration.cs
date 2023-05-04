using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sektor.API.src.Entities;

namespace Sektor.API.src.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(30);

            builder.HasMany(u => u.Memberships)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}