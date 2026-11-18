using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TallerMecanico.Datos.Interfaces;
using TallerMecanico.Datos.Repositorios;
using TallerMecanico.Entidades;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio.Interfaces;
namespace TallerMecanico.Negocio.Servicios;
public class AuthServicio(IUsuarioRepositorio repo, JwtOpciones jwt) : IAuthServicio {
 private static string Normalizar(string correo) => correo.Trim().ToLowerInvariant();
 public async Task RegistrarAsync(RegistroDto dto) {
  var correo = Normalizar(dto.Correo);
  if (await repo.ObtenerPorCorreoAsync(correo) != null) throw new ReglaNegocioException("Ya existe un usuario con ese correo.");
  if (Encoding.UTF8.GetByteCount(dto.Clave) > 72) throw new ReglaNegocioException("La clave supera 72 bytes.");
  await repo.AgregarAsync(new Usuario { Correo = correo, ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave), Rol = "Recepcionista" });
  try { await repo.GuardarAsync(); } catch (CorreoDuplicadoException) { throw new ReglaNegocioException("Ya existe un usuario con ese correo."); }
 }
 public async Task<TokenDto?> LoginAsync(LoginDto dto) {
  var usuario = await repo.ObtenerPorCorreoAsync(Normalizar(dto.Correo));
  if (usuario is null || Encoding.UTF8.GetByteCount(dto.Clave) > 72 || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash)) return null;
  var expira = DateTime.UtcNow.AddMinutes(jwt.Minutos);
  var claims = new[] { new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()), new Claim(JwtRegisteredClaimNames.Email, usuario.Correo), new Claim("role", usuario.Rol) };
  var token = new JwtSecurityToken(jwt.Emisor, jwt.Audiencia, claims, expires: expira, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Clave)), SecurityAlgorithms.HmacSha256));
  return new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), expira);
 }
}
