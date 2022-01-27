using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using TodoApi.Extensions;
using TodoApi.Models;
using TodoApi.Models.Common;
using TodoApi.Services;
var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
// Add services to the container.


services.AddEndpointsApiExplorer();

services
.ConfigureMongoDB(config)
.ConfigureAuth(config)
.ConfigureSwagger(config)
.AddScoped<TodosService>()
.AddControllers(config)
.AddSingleton<IAuthorizationHandler, HasScopeHandler>()
.AddHealthChecks();




var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
  c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
  c.OAuthClientId(config["Authentication:ClientId"]);
});

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

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


app.Run();
