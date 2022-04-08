using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.Logging;

namespace BrokenConnection
{
  class Program
  {
    static async Task Main(string[] args)
    {
      NpgsqlLogManager.Provider = new ConsoleLoggingProvider(NpgsqlLogLevel.Trace, true, true);

      var connString =
        "Host=localhost;Port=5432;Database=testdb;Include Error Detail=true";

      var optionsBuilder = new DbContextOptionsBuilder();

      optionsBuilder.UseNpgsql(
        connString,
        builder =>
        {
          builder.EnableRetryOnFailure(0, TimeSpan.FromSeconds(2), new List<string>());
          builder.CommandTimeout(2);
        });

      await using var db = new CustomDbContext(optionsBuilder.Options);

      var tests = new List<TestEntity>()
      {
        new()
        {
          Name = "test1",
        },
        new()
        {
          Name = "test2",
        }
      };
      await db.BulkCopyAsync(tests, CancellationToken.None);
    }
  }
}
