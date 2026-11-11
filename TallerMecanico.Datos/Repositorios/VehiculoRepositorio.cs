using Microsoft.EntityFrameworkCore;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Entidades;

namespace TallerMecanico.Datos.Repositorios;

public class VehiculoRepositorio(TallerMecanicoContext context) : IVehiculoRepositorio
{
    public Task<List<Vehiculo>> ListarAsync(int? clienteId = null, string? placa = null, string? marca = null)
    {
        IQueryable<Vehiculo> consulta = context.Vehiculos.AsNoTracking().Include(v => v.Cliente);
        if (clienteId.HasValue) consulta = consulta.Where(v => v.ClienteId == clienteId.Value);
        if (placa is not null) consulta = consulta.Where(v => v.Placa.Contains(placa));
        if (marca is not null) consulta = consulta.Where(v => v.Marca.Contains(marca));
        return consulta.OrderBy(v => v.Id).ToListAsync();
    }
    public Task<Vehiculo?> ObtenerAsync(int id, bool seguimiento = false) =>
        (seguimiento ? context.Vehiculos : context.Vehiculos.AsNoTracking())
            .Include(v => v.Cliente).FirstOrDefaultAsync(v => v.Id == id);
    public Task<List<Vehiculo>> ListarPorClienteAsync(int clienteId) =>
        context.Vehiculos.AsNoTracking().Include(v => v.Cliente).Where(v => v.ClienteId == clienteId)
            .OrderBy(v => v.Id).ToListAsync();
    public Task<bool> PlacaExisteAsync(string placa, int? excluirId = null) =>
        context.Vehiculos.AnyAsync(v => v.Placa == placa && (!excluirId.HasValue || v.Id != excluirId.Value));
    public Task AgregarAsync(Vehiculo vehiculo) => context.Vehiculos.AddAsync(vehiculo).AsTask();
    public void Eliminar(Vehiculo vehiculo) => context.Vehiculos.Remove(vehiculo);
    public Task GuardarAsync() => context.SaveChangesAsync();
}
