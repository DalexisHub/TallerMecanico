using TallerMecanico.Entidades.Dtos;

namespace TallerMecanico.Negocio.Interfaces;

public interface IClienteServicio
{
    Task<List<ClienteDto>> ListarAsync();
    Task<ClienteDto?> ObtenerAsync(int id);
    Task<ClienteDto> CrearAsync(ClienteCrearDto dto);
    Task<bool> ActualizarAsync(int id, ClienteActualizarDto dto);
    Task<bool> EliminarAsync(int id);
    Task<List<VehiculoDto>?> VehiculosAsync(int clienteId);
}
