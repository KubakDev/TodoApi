using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TodoApi.Extensions;

public static partial class StartupConfigurations
{

  public static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration config)
  {
    services.AddAuthentication(options =>
    {
      options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
      options.Authority = config["Authentication:Domain"];
      options.Audience = config["Authentication:Audience"];
    });

    services.AddAuthorization(options => options.AddPolicy("read:weather", policy => policy.Requirements.Add(new HasScopeRequirement("read:weather", $"https://{config["Authentication:Domain"]}/"))));
    return services;
  }

}