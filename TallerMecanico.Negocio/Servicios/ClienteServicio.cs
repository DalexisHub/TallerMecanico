using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Entidades;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio.Interfaces;

namespace TallerMecanico.Negocio.Servicios;

public class ClienteServicio(IClienteRepositorio clientes, IVehiculoRepositorio vehiculos, IMapper mapper) : IClienteServicio
{
    public async Task<List<ClienteDto>> ListarAsync() => mapper.Map<List<ClienteDto>>(await clientes.ListarAsync());
    public async Task<ClienteDto?> ObtenerAsync(int id)
    {
        var entidad = await clientes.ObtenerAsync(id);
        return entidad is null ? null : mapper.Map<ClienteDto>(entidad);
    }
    public async Task<ClienteDto> CrearAsync(ClienteCrearDto dto)
    {
        dto.Nombre = dto.Nombre.Trim();
        dto.Documento = dto.Documento.Trim();
        dto.Telefono = dto.Telefono?.Trim();
        if (await clientes.DocumentoExisteAsync(dto.Documento))
            throw new ReglaNegocioException("Ya existe un cliente con ese documento.");
        var entidad = mapper.Map<Cliente>(dto);
        await clientes.AgregarAsync(entidad);
        await GuardarClienteAsync();
        return mapper.Map<ClienteDto>(entidad);
    }
    public async Task<bool> ActualizarAsync(int id, ClienteActualizarDto dto)
    {
        var entidad = await clientes.ObtenerAsync(id, true);
        if (entidad is null) return false;
        dto.Nombre = dto.Nombre.Trim();
        dto.Documento = dto.Documento.Trim();
        dto.Telefono = dto.Telefono?.Trim();
        if (await clientes.DocumentoExisteAsync(dto.Documento, id))
            throw new ReglaNegocioException("Ya existe un cliente con ese documento.");
        mapper.Map(dto, entidad);
        await GuardarClienteAsync();
        return true;
    }
    public async Task<bool> EliminarAsync(int id)
    {
        var entidad = await clientes.ObtenerAsync(id, true);
        if (entidad is null) return false;
        if (await clientes.TieneVehiculosAsync(id))
            throw new ReglaNegocioException("No se puede eliminar un cliente que tiene vehículos.");
        clientes.Eliminar(entidad);
        await GuardarClienteAsync();
        return true;
    }
    public async Task<List<VehiculoDto>?> VehiculosAsync(int clienteId)
    {
        if (!await clientes.ExisteAsync(clienteId)) return null;
        return mapper.Map<List<VehiculoDto>>(await vehiculos.ListarPorClienteAsync(clienteId));
    }
    private async Task GuardarClienteAsync()
    {
        try { await clientes.GuardarAsync(); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new ReglaNegocioException("Ya existe un cliente con ese documento."); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 547 })
        { throw new ReglaNegocioException("No se puede eliminar un cliente que tiene vehículos."); }
    }
}
