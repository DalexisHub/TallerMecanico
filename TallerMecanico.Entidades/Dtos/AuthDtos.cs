using System.ComponentModel.DataAnnotations;
namespace TallerMecanico.Entidades.Dtos;
public class LoginDto { [Required, EmailAddress, StringLength(254)] public string Correo { get; set; } = null!; [Required, StringLength(72, MinimumLength = 8)] public string Clave { get; set; } = null!; }
public class RegistroDto : LoginDto { }
public record TokenDto(string Token, DateTime Expira);
public record PerfilDto(string Correo, string Rol);
