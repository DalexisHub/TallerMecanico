using TallerMecanico.Entidades.Dtos;

namespace TallerMecanico.Negocio.Interfaces;

public interface IVehiculoServicio
{
    Task<List<VehiculoDto>> ListarAsync(int? clienteId, string? placa, string? marca);
    Task<VehiculoDto?> ObtenerAsync(int id);
    Task<VehiculoDto> CrearAsync(VehiculoCrearDto dto);
    Task<bool> ActualizarAsync(int id, VehiculoActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}
