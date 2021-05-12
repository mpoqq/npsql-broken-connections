using Microsoft.EntityFrameworkCore;

namespace BrokenConnection
{
  public class CustomDbContext : DbContext
  {
    public DbSet<Projects> Projects { get; set; }

    public CustomDbContext(DbContextOptions options) : base(options)
    {
    }
  }

  public class Projects
  {
    public int Id { get; set; }
  }
}
