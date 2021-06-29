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
        "Host=localhost;Port=5432;Username=postgres@testdb;password=secret;database=testdb;SSL Mode=Prefer;Trust Server Certificate=true;Pooling=true;Connection Idle Lifetime=20;Minimum Pool Size=0;Maximum Pool Size=4;";

      var optionsBuilder = new DbContextOptionsBuilder();
      
      optionsBuilder.UseNpgsql(
        connString,
        builder =>
        {
          builder.EnableRetryOnFailure(0, TimeSpan.FromSeconds(2), new List<string>());
          builder.CommandTimeout(2);
        });
      
      try
      {
        await using (var db = new CustomDbContext(optionsBuilder.Options))
        {
          Console.WriteLine("First Query Starts");
          await db.Projects.ToListAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("First Query Failed");
        Console.WriteLine(ex);
      }
      
      try
      {
        await using (var db = new CustomDbContext(optionsBuilder.Options))
        {
          Console.WriteLine("Second Query Starts");
          await db.Projects.ToListAsync();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Second Query Failed");
        Console.WriteLine(ex);
      }
      
      await Task.Delay(40000);
      
      Console.WriteLine("Npgsql Team - Thank you for your hard work.");
    }
  }
}
