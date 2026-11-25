using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using XpSystem;
using XpSystem.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// ---- Keycloak / JWT ----
var keycloak = builder.Configuration.GetSection("Keycloak");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MetadataAddress = keycloak["MetadataAddress"]!;
        options.RequireHttpsMetadata = false;   // 本地走 http
        options.MapInboundClaims = false;       // 保住 "sub" 这个 claim 名

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = keycloak["Issuer"]!,
            ValidateAudience = true,
            ValidAudience = keycloak["Audience"]!,
            NameClaimType = "preferred_username"
        };
    });

builder.Services.AddAuthorization();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
