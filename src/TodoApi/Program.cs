using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using TodoApi.Extensions;
using TodoApi.Models;
using TodoApi.Models.Common;
using TodoApi.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
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
                ); ;


builder.Services.AddEndpointsApiExplorer();

builder.Services
.ConfigureMongoDB(builder.Configuration)
.ConfigureAuth(builder.Configuration)
.AddScoped<TodosService>()
.AddSingleton<IAuthorizationHandler, HasScopeHandler>()
.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.Authority = builder.Configuration["Authentication:Domain"];
  options.Audience = builder.Configuration["Authentication:Audience"];
});



builder.Services.AddSwaggerGen(c =>
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
        AuthorizationUrl = new Uri(builder.Configuration["Authentication:Domain"] + "authorize?audience=" + builder.Configuration["Authentication:Audience"])
      }
    }
  });

  c.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
  c.OAuthClientId(builder.Configuration["Authentication:ClientId"]);
});

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
