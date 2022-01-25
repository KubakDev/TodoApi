using Microsoft.OpenApi.Models;

namespace TodoApi.Extensions;


public static partial class StartupConfigurations
{

  public static IServiceCollection ConfigureSwagger(this IServiceCollection services, IConfiguration config)
  {
    services.AddSwaggerGen(c =>
{

c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
  Name = "Authorization",
  In = ParameterLocation.Header,
  Type = SecuritySchemeType.OAuth2,
  Flows = new OpenApiOAuthFlows
  {
    Implicit = new OpenApiOAuthFlow
    {
      Scopes = new Dictionary<string, string>
              {
                    { "openid", "Open Id" }
              },
      AuthorizationUrl = new Uri(config["Authentication:Domain"] + "authorize?audience=" + config["Authentication:Audience"])
    }
  }
});

c.OperationFilter<SecurityRequirementsOperationFilter>();
});

    return services;
  }


}