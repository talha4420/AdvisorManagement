using Microsoft.EntityFrameworkCore;
using Advisor.Domain.Models;


namespace Advisor.Core.DBContexts;
public class AdvisorDBContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AdvisorProfile> AdvisorProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdvisorProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.SIN)
                  .IsRequired()
                  .IsUnicode()
                  .HasMaxLength(9);

            entity.HasIndex(e => e.SIN)
              .IsUnique();

            entity.Property(e => e.Address)
                  .HasMaxLength(255);

            entity.Property(e => e.PhoneNumber)
                  .HasMaxLength(10);

            entity.Property(e => e.HealthStatus)
                  .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}