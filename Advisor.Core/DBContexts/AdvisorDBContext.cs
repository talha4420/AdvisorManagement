using Microsoft.EntityFrameworkCore;
using Advisor.Domain.Models;
using Advisor.Domain.DomainServices;


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

      try{
            HealthStatusGeneratorService healthStatusGenerator = new HealthStatusGeneratorService();
            var advisorProfiles = new[]
            {
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "John Doe", SIN = "123456789", Address = "123 Main St", PhoneNumber = "1234567890" },
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Jane Smith", SIN = "987654321", Address = "456 Elm St", PhoneNumber = "9876543210" },
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Alice Johnson", SIN = "111111111", Address = "789 Oak St", PhoneNumber = "1111111110" },
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Bob Brown", SIN = "222222222", Address = "101 Pine St", PhoneNumber = "2222222220" },
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Charlie White", SIN = "333333333", Address = "202 Maple St", PhoneNumber = "3333333330" },
                new AdvisorProfile { Id = Guid.NewGuid(), FullName = "David Black", SIN = "444444444", Address = "303 Birch St", PhoneNumber = "4444444440" },
                  new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Eve Green", SIN = "555555555", Address = "404 Cedar St", PhoneNumber = "5555555550" },
                  new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Frank Red", SIN = "666666666", Address = "505 Walnut St", PhoneNumber = "6666666660" },
                  new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Grace Blue", SIN = "777777777", Address = "606 Spruce St", PhoneNumber = "7777777770" },
                  new AdvisorProfile { Id = Guid.NewGuid(), FullName = "Hank Yellow", SIN = "888888888", Address = "707 Pine St", PhoneNumber = "8888888880" }
            };

            foreach (var profile in advisorProfiles)
            {
                  profile.UpdateHealthStatus(healthStatusGenerator);
            }

            modelBuilder.Entity<AdvisorProfile>().HasData(advisorProfiles);
      }catch(Exception e){
              Console.WriteLine(e.Message);
      }
        
        base.OnModelCreating(modelBuilder);
    }
}