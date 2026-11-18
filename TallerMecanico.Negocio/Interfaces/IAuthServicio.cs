using TallerMecanico.Entidades.Dtos;
namespace TallerMecanico.Negocio.Interfaces;
public interface IAuthServicio { Task RegistrarAsync(RegistroDto dto); Task<TokenDto?> LoginAsync(LoginDto dto); }
