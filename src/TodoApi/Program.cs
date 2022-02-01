using Microsoft.AspNetCore.Authorization;
using TodoApi.Services;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
// Add services to the container.



services
.ConfigureMongoDB(config)
.ConfigureAuth(config)
.ConfigureSwagger(config)
.AddScoped<TodosService>()
.AddControllers(config)
.AddSingleton<IAuthorizationHandler, HasScopeHandler>()
.ConfigureHealthChecks(config)
.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  string prefix = String.Empty;
  if (builder.Environment.IsProduction())
    prefix = "/api";

  c.SwaggerEndpoint($"{prefix}/swagger/v1/swagger.json", "API");

  c.OAuthClientId(config["Authentication:ClientId"]);
});

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.AddHealthChecks(config);




app.Run();
