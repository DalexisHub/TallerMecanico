using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Entidades;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio.Interfaces;

namespace TallerMecanico.Negocio.Servicios;

public class VehiculoServicio(IVehiculoRepositorio vehiculos, IClienteRepositorio clientes, IMapper mapper) : IVehiculoServicio
{
    public async Task<List<VehiculoDto>> ListarAsync(int? clienteId, string? placa, string? marca) =>
        mapper.Map<List<VehiculoDto>>(await vehiculos.ListarAsync(clienteId, placa?.Trim().ToUpperInvariant(), marca?.Trim()));
    public async Task<VehiculoDto?> ObtenerAsync(int id)
    {
        var entidad = await vehiculos.ObtenerAsync(id);
        return entidad is null ? null : mapper.Map<VehiculoDto>(entidad);
    }
    public async Task<VehiculoDto> CrearAsync(VehiculoCrearDto dto)
    {
        Normalizar(dto);
        await ValidarAsync(dto.Placa, dto.ClienteId);
        var entidad = mapper.Map<Vehiculo>(dto);
        await vehiculos.AgregarAsync(entidad);
        await GuardarVehiculoAsync();
        return mapper.Map<VehiculoDto>((await vehiculos.ObtenerAsync(entidad.Id))!);
    }
    public async Task<bool> ActualizarAsync(int id, VehiculoActualizarDto dto)
    {
        var entidad = await vehiculos.ObtenerAsync(id, true);
        if (entidad is null) return false;
        Normalizar(dto);
        await ValidarAsync(dto.Placa, dto.ClienteId, id);
        mapper.Map(dto, entidad);
        await GuardarVehiculoAsync();
        return true;
    }
    public async Task<bool> EliminarAsync(int id)
    {
        var entidad = await vehiculos.ObtenerAsync(id, true);
        if (entidad is null) return false;
        vehiculos.Eliminar(entidad);
        await vehiculos.GuardarAsync();
        return true;
    }
    private async Task ValidarAsync(string placa, int clienteId, int? excluirId = null)
    {
        if (await vehiculos.PlacaExisteAsync(placa, excluirId))
            throw new ReglaNegocioException("Ya existe un vehículo con esa placa.");
        if (!await clientes.ExisteAsync(clienteId))
            throw new ReglaNegocioException("El cliente indicado no existe.");
    }
    private async Task GuardarVehiculoAsync()
    {
        try { await vehiculos.GuardarAsync(); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new ReglaNegocioException("Ya existe un vehículo con esa placa."); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 547 })
        { throw new ReglaNegocioException("El cliente indicado no existe."); }
    }
    private static void Normalizar(VehiculoCrearDto dto)
    { dto.Placa = dto.Placa.Trim().ToUpperInvariant(); dto.Marca = dto.Marca.Trim(); dto.Modelo = dto.Modelo.Trim(); }
    private static void Normalizar(VehiculoActualizarDto dto)
    { dto.Placa = dto.Placa.Trim().ToUpperInvariant(); dto.Marca = dto.Marca.Trim(); dto.Modelo = dto.Modelo.Trim(); }
}
