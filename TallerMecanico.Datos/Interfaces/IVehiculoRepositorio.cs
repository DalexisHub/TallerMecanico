using TallerMecanico.Entidades;

namespace TallerMecanico.Datos.Interfaces;

public interface IVehiculoRepositorio
{
    Task<List<Vehiculo>> ListarAsync(int? clienteId = null, string? placa = null, string? marca = null);
    Task<Vehiculo?> ObtenerAsync(int id, bool seguimiento = false);
    Task<List<Vehiculo>> ListarPorClienteAsync(int clienteId);
    Task<bool> PlacaExisteAsync(string placa, int? excluirId = null);
    Task AgregarAsync(Vehiculo vehiculo);
    void Eliminar(Vehiculo vehiculo);
    Task GuardarAsync();
}
