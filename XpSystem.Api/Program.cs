using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using XpSystem;
using XpSystem.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using XpSystem.Api.Auth;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// ---- Keycloak / JWT ----
// var keycloak = builder.Configuration.GetSection("Keycloak");

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.MetadataAddress = keycloak["MetadataAddress"]!;
//         options.RequireHttpsMetadata = false;   // 本地走 http
//         options.MapInboundClaims = false;       // 保住 "sub" 这个 claim 名

//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = keycloak["Issuer"]!,
//             ValidateAudience = true,
//             ValidAudience = keycloak["Audience"]!,
//             NameClaimType = "preferred_username"
//         };
//     });

// ---- Keycloak / JWT ----
var keycloak = builder.Configuration.GetSection("Keycloak");
var jwksUrl = keycloak["JwksAddress"]!;

// 启动时从内部地址取一次 Keycloak 的签名公钥（容器内解析不了 *.localhost）
IEnumerable<SecurityKey>? signingKeys = null;
using (var http = new HttpClient())
{
    for (var attempt = 1; attempt <= 10 && signingKeys is null; attempt++)
    {
        try
        {
            var jwksJson = await http.GetStringAsync(jwksUrl);
            signingKeys = new JsonWebKeySet(jwksJson).GetSigningKeys();
        }
        catch (Exception)
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}

if (signingKeys is null || !signingKeys.Any())
    throw new InvalidOperationException($"Kunne ikke hente JWKS fra {jwksUrl}");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = keycloak["Issuer"]!,
            ValidateAudience = true,
            ValidAudience = keycloak["Audience"]!,
            ValidateLifetime = true,
            IssuerSigningKeys = signingKeys,
            NameClaimType = "preferred_username"
        };
    });


builder.Services.AddAuthorization();

builder.Services.AddHealthChecks();

// OpenAPI document + Scalar UI
builder.Services.AddOpenApi(options =>
{
    var issuer = keycloak["Issuer"]!;   // http://keycloak.localhost/realms/xpsystem

    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["oauth2"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{issuer}/protocol/openid-connect/auth"),
                    TokenUrl = new Uri($"{issuer}/protocol/openid-connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        ["openid"] = "OpenID Connect",
                        ["profile"] = "User profile"
                    }
                }
            }
        };

        // 让所有端点默认要求这个 scheme，Scalar 才会自动带 Authorization 头
        document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("oauth2", document)] = ["openid", "profile"]
            }
        ];

        return Task.CompletedTask;
    });
});


// EF Core + PostgreSQL
builder.Services.AddDbContext<XpSystemDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<PlayerService>();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();

builder.Services.AddHttpContextAccessor();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<XpSystemDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();            // OpenAPI 文档: /openapi/v1.json
//app.MapScalarApiReference(); // Scalar UI:    /scalar/v1
app.MapScalarApiReference(options =>
{
    options
        .WithTitle("XpSystem API")
        .AddPreferredSecuritySchemes("oauth2")
        .AddAuthorizationCodeFlow("oauth2", flow =>
        {
            flow.ClientId = "xp-api";
            flow.Pkce = Pkce.Sha256;
            flow.SelectedScopes = ["openid", "profile"];
        });
});


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
