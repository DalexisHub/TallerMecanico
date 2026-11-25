using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Datos.Repositorios;
using TallerMecanico.Negocio;
using TallerMecanico.Negocio.Interfaces;
using TallerMecanico.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);
if (int.TryParse(builder.Configuration["PORT"], out var port) && port is > 0 and <= 65535)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOpciones>() ?? new JwtOpciones();
if (string.IsNullOrWhiteSpace(jwt.Clave) || jwt.Clave.Length < 32 || Encoding.UTF8.GetByteCount(jwt.Clave) < 32 || string.IsNullOrWhiteSpace(jwt.Emisor) || string.IsNullOrWhiteSpace(jwt.Audiencia) || jwt.Minutos <= 0 || jwt.Minutos > 1440)
    throw new InvalidOperationException("Configure Jwt:Clave (mínimo 32 caracteres), Emisor, Audiencia y Minutos (1 a 1440) mediante configuración segura.");
builder.Services.AddSingleton(jwt);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
 options.MapInboundClaims = false;
 options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuer = true, ValidIssuer = jwt.Emisor, ValidateAudience = true, ValidAudience = jwt.Audiencia, ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)), ValidateLifetime = true, RequireExpirationTime = true, ClockSkew = TimeSpan.Zero, RoleClaimType = "role", NameClaimType = "email", ValidAlgorithms = [SecurityAlgorithms.HmacSha256] };
});
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(options => {
 options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme { Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT" });
 options.OperationFilter<BearerOperationFilter>();
});
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IAuthServicio, AuthServicio>();
builder.Services.AddDbContext<TallerMecanicoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TallerMecanico")));
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IVehiculoRepositorio, VehiculoRepositorio>();
builder.Services.AddScoped<IClienteServicio, ClienteServicio>();
builder.Services.AddScoped<IVehiculoServicio, VehiculoServicio>();
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

var app = builder.Build();
app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>().AssertConfigurationIsValid();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
