using TallerMecanico.Entidades;
namespace TallerMecanico.Datos.Interfaces;
public interface IUsuarioRepositorio { Task<Usuario?> ObtenerPorCorreoAsync(string correo); Task AgregarAsync(Usuario usuario); Task GuardarAsync(); }
