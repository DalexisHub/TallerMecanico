using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Datos.Repositorios;
using TallerMecanico.Negocio;
using TallerMecanico.Negocio.Interfaces;
using TallerMecanico.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
