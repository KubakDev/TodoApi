using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using TodoApi.Settings;
using TodoApi.Models;

namespace OrbitFoodApi.Extensions
{
  public static partial class StartupConfigurations
  {
    public static IServiceCollection ConfigureMongoDB(this IServiceCollection services, IConfiguration configuration)
    {
      configuration = configuration.GetSection(MongoDBOptions.Config);

      services
          .AddOptions<MongoDBOptions>()
          .Bind(configuration)
          .ValidateDataAnnotations();

      ConfigureBson();

      return services.AddSingleton(ClientImplementationFactory)
                     .AddSingleton(DatabaseImplementationFactory)
                     .AddSingleton(sp => CollectionImplementationFactory<Todo>(sp, "todos"))
                     .AddSingleton(sp => CollectionImplementationFactory<User>(sp, "users"))
                     ;
    }

    private static IMongoClient ClientImplementationFactory(IServiceProvider serviceProvider)
    {
      var config = serviceProvider.GetRequiredService<IOptions<MongoDBOptions>>();

      var clientSetttings = MongoClientSettings.FromConnectionString(config.Value.ConnectionString);

#if DEBUG
      var logger = serviceProvider.GetRequiredService<ILogger<MongoClient>>();
      var jsonWriterSettings = new JsonWriterSettings()
      {
        Indent = true,
        OutputMode = JsonOutputMode.Shell
      };

      clientSetttings.ClusterConfigurator = cb =>
      {
        cb.Subscribe<CommandStartedEvent>(e =>
              {
                logger.LogDebug("{commandName}: => \n{commandJson}", e.CommandName, e.Command.ToJson(jsonWriterSettings));
              });
      };
#endif

      return new MongoClient(clientSetttings);
    }

    private static IMongoDatabase DatabaseImplementationFactory(IServiceProvider serviceProvider)
    {
      var config = serviceProvider.GetRequiredService<IOptions<MongoDBOptions>>();
      var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();

      return mongoClient.GetDatabase(config.Value.DatabaseName);
    }

    private static IMongoCollection<T> CollectionImplementationFactory<T>(IServiceProvider serviceProvider, in string collectionName)
    {
      var mongoDatabase = serviceProvider.GetRequiredService<IMongoDatabase>();

      return mongoDatabase.GetCollection<T>(collectionName);
    }

    private static void ConfigureBson()
    {
      var conventionPack = new ConventionPack
            {
                new IgnoreIfNullConvention(true),
                new CamelCaseElementNameConvention(),
                new IgnoreExtraElementsConvention(true)
            };

      ConventionRegistry.Register("default", conventionPack, t => true);
    }

  }
}