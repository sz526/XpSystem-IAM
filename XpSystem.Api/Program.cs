using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using XpSystem;
using XpSystem.Api.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// OpenAPI document + Scalar UI
builder.Services.AddOpenApi();

// EF Core + PostgreSQL
builder.Services.AddDbContext<XpSystemDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<PlayerService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<XpSystemDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();            // OpenAPI 文档: /openapi/v1.json
app.MapScalarApiReference(); // Scalar UI:    /scalar/v1

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
