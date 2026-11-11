using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.Entidades.Dtos;

public class ClienteCrearDto
{
    private string nombre = null!;
    private string documento = null!;
    private string? telefono;
    [Required, StringLength(100), RegularExpression(@".*\S.*", ErrorMessage = "El nombre no puede estar vacío.")]
    public string Nombre { get => nombre; set => nombre = value?.Trim()!; }
    [Required, StringLength(20), RegularExpression(@".*\S.*", ErrorMessage = "El documento no puede estar vacío.")]
    public string Documento { get => documento; set => documento = value?.Trim()!; }
    [StringLength(20)]
    public string? Telefono { get => telefono; set => telefono = value?.Trim(); }
}
