using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using TodoApi.Settings;

namespace TodoApi.Extensions
{
  public static partial class StartupConfigurations
  {

    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
      var mongodb = configuration.GetSection(MongoDBOptions.Config);

      services
          .AddHealthChecks()
          .AddMongoDb(name: "mongodb",
                      mongoDatabaseName: mongodb[nameof(MongoDBOptions.DatabaseName)],
                      mongodbConnectionString: mongodb[nameof(MongoDBOptions.ConnectionString)],
                      tags: new[] { "ready" },
                      timeout: TimeSpan.FromSeconds(3));

      return services;
    }
    public static IApplicationBuilder AddHealthChecks(this WebApplication app, IConfiguration configuration)
    {

      app.MapHealthChecks("/health/ready", new HealthCheckOptions
      {
        Predicate = (check) => check.Tags.Contains("ready"),
        ResponseWriter = async (context, report) =>
        {
          var result = JsonSerializer.Serialize(new
          {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry =>
            new
            {
              name = entry.Key,
              status = entry.Value.Status.ToString(),
              exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "",
              duration = entry.Value.Duration.ToString()
            })
          });
          context.Response.ContentType = MediaTypeNames.Application.Json;
          await context.Response.WriteAsync(result);
        }
      });

      app.MapHealthChecks("/health/live", new HealthCheckOptions
      {
        Predicate = (_) => false
      });

      return app;
    }

  }
}