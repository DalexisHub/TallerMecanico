using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Entidades;

namespace TallerMecanico.Datos.Repositorios;

public class ClienteRepositorio(TallerMecanicoContext context) : IClienteRepositorio
{
    public Task<List<Cliente>> ListarAsync() => context.Clientes.AsNoTracking().OrderBy(c => c.Id).ToListAsync();
    public Task<Cliente?> ObtenerAsync(int id, bool seguimiento = false) =>
        (seguimiento ? context.Clientes : context.Clientes.AsNoTracking()).FirstOrDefaultAsync(c => c.Id == id);
    public Task<bool> ExisteAsync(int id) => context.Clientes.AnyAsync(c => c.Id == id);
    public Task<bool> DocumentoExisteAsync(string documento, int? excluirId = null) =>
        context.Clientes.AnyAsync(c => c.Documento == documento && (!excluirId.HasValue || c.Id != excluirId.Value));
    public Task<bool> TieneVehiculosAsync(int id) => context.Vehiculos.AnyAsync(v => v.ClienteId == id);
    public Task AgregarAsync(Cliente cliente) => context.Clientes.AddAsync(cliente).AsTask();
    public void Eliminar(Cliente cliente) => context.Clientes.Remove(cliente);
    public Task GuardarAsync() => context.SaveChangesAsync();
}
