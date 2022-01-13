using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using OrbitFoodApi.Extensions;
using TodoApi.Models;
using TodoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureMongoDB(builder.Configuration);

builder.Services.AddSingleton<TodosService>();
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
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.Authority = builder.Configuration["Authentication:Domain"];
  options.Audience = builder.Configuration["Authentication:Audience"];
});
builder.Services.AddAuthorization(options =>
          {
            options.AddPolicy("read:weather", policy => policy.Requirements.Add(new HasScopeRequirement("read:weather", $"https://{builder.Configuration["Auth0:Domain"]}/")));
          });

builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

builder.Services.AddControllers(options =>
{
  var policy = new AuthorizationPolicyBuilder()
      .RequireAuthenticatedUser()
      .Build();

  options.Filters.Add(new AuthorizeFilter(policy));
})
     .AddNewtonsoftJson(options =>
     {
       options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
     }).AddJsonOptions(options =>
                   {
                     // Use the default property (Pascal) casing.
                     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                   }
                ); ;


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
