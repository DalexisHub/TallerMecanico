using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Entidades;
namespace TallerMecanico.Datos.Repositorios;
public class UsuarioRepositorio(TallerMecanicoContext db) : IUsuarioRepositorio {
 public Task<Usuario?> ObtenerPorCorreoAsync(string correo) => db.Usuarios.FirstOrDefaultAsync(x => x.Correo == correo);
 public async Task AgregarAsync(Usuario usuario) => await db.Usuarios.AddAsync(usuario);
 public async Task GuardarAsync() { try { await db.SaveChangesAsync(); } catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 }) { throw new CorreoDuplicadoException(); } }
}
public class CorreoDuplicadoException : Exception;
