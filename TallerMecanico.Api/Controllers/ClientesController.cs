using Microsoft.AspNetCore.Mvc;
using TallerMecanico.Entidades.Dtos;
using TallerMecanico.Negocio;
using TallerMecanico.Negocio.Interfaces;

namespace TallerMecanico.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController(IClienteServicio servicio) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ClienteDto>), 200)]
    public async Task<ActionResult<List<ClienteDto>>> Listar() => Ok(await servicio.ListarAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClienteDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<ClienteDto>> Obtener(int id)
    {
        var dto = await servicio.ObtenerAsync(id);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClienteDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<ActionResult<ClienteDto>> Crear(ClienteCrearDto dto)
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
    public async Task<IActionResult> Actualizar(int id, ClienteActualizarDto dto)
    {
        try { return await servicio.ActualizarAsync(id, dto) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException e) { return Conflict(Problema(e)); }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 409)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try { return await servicio.EliminarAsync(id) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException e) { return Conflict(Problema(e)); }
    }

    [HttpGet("{clienteId:int}/vehiculos")]
    [ProducesResponseType(typeof(List<VehiculoDto>), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    public async Task<ActionResult<List<VehiculoDto>>> Vehiculos(int clienteId)
    {
        var lista = await servicio.VehiculosAsync(clienteId);
        return lista is null ? NotFound() : Ok(lista);
    }

    private static ProblemDetails Problema(Exception e) => new() { Status = 409, Title = "Conflicto de negocio", Detail = e.Message };
}
