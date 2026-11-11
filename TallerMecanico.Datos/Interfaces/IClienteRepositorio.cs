using TallerMecanico.Entidades;

namespace TallerMecanico.Datos.Interfaces;

public interface IClienteRepositorio
{
    Task<List<Cliente>> ListarAsync();
    Task<Cliente?> ObtenerAsync(int id, bool seguimiento = false);
    Task<bool> ExisteAsync(int id);
    Task<bool> DocumentoExisteAsync(string documento, int? excluirId = null);
    Task<bool> TieneVehiculosAsync(int id);
    Task AgregarAsync(Cliente cliente);
    void Eliminar(Cliente cliente);
    Task GuardarAsync();
}
