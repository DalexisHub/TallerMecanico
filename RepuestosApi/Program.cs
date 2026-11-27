using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using RepuestosApi;

var builder = WebApplication.CreateBuilder(args);
if (int.TryParse(builder.Configuration["PORT"], out var port) && port is > 0 and <= 65535)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var clave = builder.Configuration["Jwt:Clave"];
var emisor = builder.Configuration["Jwt:Emisor"];
var audiencia = builder.Configuration["Jwt:Audiencia"];
if (string.IsNullOrWhiteSpace(clave) || Encoding.UTF8.GetByteCount(clave) < 32 ||
    string.IsNullOrWhiteSpace(emisor) || string.IsNullOrWhiteSpace(audiencia))
    throw new InvalidOperationException("Configure Jwt:Clave (mínimo 32 bytes), Jwt:Emisor y Jwt:Audiencia.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(clave)),
            ValidateIssuer = true,
            ValidIssuer = emisor,
            ValidateAudience = true,
            ValidAudience = audiencia,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            ClockSkew = TimeSpan.Zero,
            ValidAlgorithms = [SecurityAlgorithms.HmacSha256]
        };
    });
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser().Build());

var repuestos = new List<Repuesto>
{
    new(1, "FIL-ACE-001", "Filtro de aceite", "Bosch", "Filtros", 38.90m, 24),
    new(2, "PAS-FRE-002", "Pastillas de freno delanteras", "Brembo", "Frenos", 179.50m, 12),
    new(3, "BUJ-ENC-003", "Juego de bujías", "NGK", "Encendido", 94.00m, 18),
    new(4, "BAT-ELC-004", "Batería 12 V 60 Ah", "Etna", "Eléctrico", 389.00m, 7),
    new(5, "FIL-AIR-005", "Filtro de aire", "Mann-Filter", "Filtros", 52.50m, 20),
    new(6, "COR-DIS-006", "Correa de distribución", "Gates", "Motor", 215.00m, 9),
    new(7, "AMO-SUS-007", "Amortiguador delantero", "Monroe", "Suspensión", 279.90m, 6),
    new(8, "LIQ-FRE-008", "Líquido de frenos DOT 4", "Bosch", "Frenos", 42.00m, 30)
};

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
var api = app.MapGroup("/api/repuestos").RequireAuthorization();
api.MapGet("", () => Results.Ok(repuestos));
api.MapGet("/{id:int}", (int id) =>
    repuestos.Find(repuesto => repuesto.Id == id) is { } repuesto
        ? Results.Ok(repuesto)
        : Results.NotFound());
app.Run();
