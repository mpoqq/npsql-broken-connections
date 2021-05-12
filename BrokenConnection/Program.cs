using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql.Logging;

namespace BrokenConnection
{
  class Program
  {
    static async Task Main(string[] args)
    {
      NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Trace, true, true);

      // Use Azure PostgreSQL or any other provider which does not support Cancel Request
      var connString =
        "Host=localhost;Port=5432;Username=postgres@testdb;password=secret;database=testdb;SSL Mode=Prefer;Trust Server Certificate=true;Pooling=true;Connection Idle Lifetime=180;Minimum Pool Size=0;Maximum Pool Size=4;";

      var optionsBuilder = new DbContextOptionsBuilder();

      optionsBuilder.UseNpgsql(
        connString,
        builder =>
        {
          builder.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), new List<string>());
          builder.CommandTimeout(2);
        });

      try
      {
        await using (var db = new CustomDbContext(optionsBuilder.Options))
        {
          Console.WriteLine("First Query Starts");
          await db.Projects.FromSqlRaw("select pg_sleep(200);").ToListAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("First Query Failed - Timeouts");
        Console.WriteLine(ex);
      }

      try
      {
        await using (var db = new CustomDbContext(optionsBuilder.Options))
        {
          Console.WriteLine("Second Query Starts");
          await db.Projects.FromSqlRaw("select pg_sleep(200);").ToListAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Second Query Failed - Pool Exhausted");
        Console.WriteLine(ex);
      }

      Console.WriteLine("Npgsql Team - Thank you for your hard work.");
    }
  }
}
