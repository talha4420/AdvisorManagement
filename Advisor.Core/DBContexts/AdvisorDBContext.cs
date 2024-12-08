using Microsoft.EntityFrameworkCore;
using Advisor.Domain.Models;


namespace Advisor.Core.DBContexts;
public class AdvisorDBContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<AdvisorProfile> AdvisorProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }

}