using Microsoft.EntityFrameworkCore;

namespace BrokenConnection
{
  public class CustomDbContext : DbContext
  {
    public DbSet<TestEntity> TestEntity { get; set; }

    public CustomDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<TestEntity>().UseXminAsConcurrencyToken();
    }
  }

  public class TestEntity
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public uint xmin { get; set; }
  }
}
