using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using TodoApi.Models.Common;

namespace TodoApi.Extensions;

public static partial class StartupConfigurations
{



  public static IServiceCollection AddControllers(this IServiceCollection services, IConfiguration config)
  {

    services.AddControllers(
      options =>
    {
      var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();

      options.Filters.Add(new AuthorizeFilter(policy));
    }
    ).AddJsonOptions(options =>
                       {
                         // Use the default property (Pascal) casing.
                         options.JsonSerializerOptions.Converters.Add(new BsonDocumentConverter());
                         options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                       }
                    );

    return services;
  }
}