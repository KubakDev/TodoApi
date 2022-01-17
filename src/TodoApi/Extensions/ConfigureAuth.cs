using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TodoApi.Extensions;

public static partial class StartupConfigurations
{

  public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration config)
  {

    return services.AddAuthorization(options => options.AddPolicy("read:weather", policy => policy.Requirements.Add(new HasScopeRequirement("read:weather", $"https://{config["Authentication:Domain"]}/"))));

  }

}