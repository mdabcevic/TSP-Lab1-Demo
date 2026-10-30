using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TSP_LAB1.Data;
using TSP_LAB1.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<ShopDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Shop") ?? throw new InvalidOperationException("Connection string 'Shop' nije konfiguriran.");
    options.UseNpgsql(connectionString);
});

// Omogucuje drugim servisima (Auth, Token...) pristup JWT konfiguraciji putem DI.
builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))

    // Enforce pravila i ograničenja (required, range...)
    .ValidateDataAnnotations()
    .Validate(options => Encoding.UTF8.GetByteCount(options.SigningKey) >= 32, "JWT signing key must contain at least 32 bytes.")

    // Fail Securely princip - bolje je zaustaviti aplikaciju s neispravnom sigurnosnom konfiguracijom nego problem otkriti tek prilikom prvog autentikacijskog zahtjeva
    .ValidateOnStart();

// TODO: 1. zadatak - postavite odgovarajuću JWT konfiguraciju u appsettubgs.json

var jwt = builder.Configuration  // Ovu instancu JWT tokena je potrebno koristiti za centralnu konfiguraciju middleware, prije nego sto aplikacije postane dostupna.

    .GetRequiredSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? throw new InvalidOperationException("JWT configuration is missing."); // Nakon 1. zadatka, greška će nestati

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateLifetime = true,
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                ClockSkew = TimeSpan.Zero,
                NameClaimType = "sub",
                RoleClaimType = "role"
            };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var principal = context.Principal;

                var sub = principal?.FindFirstValue("sub");
                var sid = principal?.FindFirstValue("sid");
                var version = principal?.FindFirstValue("av");
                var role = principal?.FindFirstValue("role");

                if (!Guid.TryParse(sub, out var userId) || !Guid.TryParse(sid, out var sessionId) || !int.TryParse(version, out var authVersion) || string.IsNullOrWhiteSpace(role))
                {
                    context.Fail("Invalid token claims.");
                    return;
                }

                
            }
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
