using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
if (int.TryParse(builder.Configuration["PORT"], out var port) && port is > 0 and <= 65535)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: false)
    .AddEnvironmentVariables();

// En producción los hosts se suministran desde el entorno, sin editar ocelot.json.
if (builder.Environment.IsProduction())
{
    var tallerHost = builder.Configuration["Downstream:TallerMecanico:Host"];
    var repuestosHost = builder.Configuration["Downstream:Repuestos:Host"];
    var baseUrl = builder.Configuration["Gateway:BaseUrl"];
    if (string.IsNullOrWhiteSpace(tallerHost) || string.IsNullOrWhiteSpace(repuestosHost) ||
        string.IsNullOrWhiteSpace(baseUrl) || !Uri.TryCreate(baseUrl, UriKind.Absolute, out var gatewayUrl) ||
        gatewayUrl.Scheme != Uri.UriSchemeHttps)
        throw new InvalidOperationException("Configure Downstream__TallerMecanico__Host, Downstream__Repuestos__Host y Gateway__BaseUrl (HTTPS) para producción.");

    var overrides = new Dictionary<string, string?> { ["GlobalConfiguration:BaseUrl"] = baseUrl };
    for (var i = 0; builder.Configuration.GetSection("Routes").GetSection(i.ToString()).Exists(); i++)
    {
        var prefix = $"Routes:{i}";
        var routePort = builder.Configuration[$"{prefix}:DownstreamHostAndPorts:0:Port"];
        overrides[$"{prefix}:DownstreamScheme"] = "https";
        overrides[$"{prefix}:DownstreamHostAndPorts:0:Host"] = routePort == "5056" ? repuestosHost : tallerHost;
        overrides[$"{prefix}:DownstreamHostAndPorts:0:Port"] = "443";
    }
    builder.Configuration.AddInMemoryCollection(overrides);
}

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
builder.Services.AddAuthorization();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
await app.UseOcelot();
app.Run();
