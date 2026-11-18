using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio;
using TallerMecanico.Negocio.Interfaces;
namespace TallerMecanico.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class AuthController(IAuthServicio auth) : ControllerBase {
 [AllowAnonymous, HttpPost("registro")] public async Task<IActionResult> Registro(RegistroDto dto) { try { await auth.RegistrarAsync(dto); return StatusCode(201); } catch (ReglaNegocioException e) { return Conflict(new ProblemDetails { Status = 409, Detail = e.Message }); } }
 [AllowAnonymous, HttpPost("login")] public async Task<ActionResult<TokenDto>> Login(LoginDto dto) { var token = await auth.LoginAsync(dto); return token is null ? Unauthorized() : Ok(token); }
 [Authorize, HttpGet("perfil")] public ActionResult<PerfilDto> Perfil() => Ok(new PerfilDto(User.FindFirstValue("email")!, User.FindFirstValue("role")!));
}
