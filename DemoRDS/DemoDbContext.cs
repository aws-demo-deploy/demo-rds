using Microsoft.EntityFrameworkCore;

namespace DemoRDS;

public class DemoDbContext : DbContext
{
    public DbSet<DemoModel> DemoModels => Set<DemoModel>();

    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DemoModel>().HasData(
            new DemoModel { Id = 1, Text = "test1" },
            new DemoModel { Id = 2, Text = "test2" }
        );
        base.OnModelCreating(modelBuilder);
    }
}