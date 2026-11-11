using Microsoft.AspNetCore.Mvc;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio;
using TallerMecanico.Negocio.Interfaces;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehiculosController(IVehiculoServicio servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<VehiculoDto>), 200)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    public async Task<ActionResult<List<VehiculoDto>>> Listar([FromQuery] int? clienteId, [FromQuery] string? placa, [FromQuery] string? marca)
    {
        if (clienteId is <= 0) ModelState.AddModelError(nameof(clienteId), "Debe ser mayor que cero.");
        if (placa is not null && string.IsNullOrWhiteSpace(placa)) ModelState.AddModelError(nameof(placa), "No puede estar vacía.");
        if (marca is not null && string.IsNullOrWhiteSpace(marca)) ModelState.AddModelError(nameof(marca), "No puede estar vacía.");
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        return Ok(await servicio.ListarAsync(clienteId, placa, marca));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VehiculoDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<VehiculoDto>> Obtener(int id)
    {
        var dto = await servicio.ObtenerAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VehiculoDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult<VehiculoDto>> Crear(VehiculoCrearDto dto)
    {
        try
        {
            var creado = await servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException e) { return Conflict(Problema(e)); }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Actualizar(int id, VehiculoActualizarDto dto)
    {
        try { return await servicio.ActualizarAsync(id, dto) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException e) { return Conflict(Problema(e)); }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<IActionResult> Eliminar(int id) => await servicio.EliminarAsync(id) ? NoContent() : NotFound();

    private static ProblemDetails Problema(Exception e) => new() { Status = 409, Title = "Conflicto de negocio", Detail = e.Message };
}
