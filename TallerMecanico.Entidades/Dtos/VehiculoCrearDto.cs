using System.ComponentModel.DataAnnotations;

namespace TallerMecanico.Entidades.Dtos;

public class VehiculoCrearDto
{
    private string placa = null!;
    private string marca = null!;
    private string modelo = null!;
    [Required, StringLength(15), RegularExpression(@".*\S.*", ErrorMessage = "La placa no puede estar vacía.")]
    public string Placa { get => placa; set => placa = value?.Trim().ToUpperInvariant()!; }
    [Required, StringLength(50), RegularExpression(@".*\S.*")]
    public string Marca { get => marca; set => marca = value?.Trim()!; }
    [Required, StringLength(50), RegularExpression(@".*\S.*")]
    public string Modelo { get => modelo; set => modelo = value?.Trim()!; }
    [Range(1900, 2100)] public int Anio { get; set; }
    [Range(1, int.MaxValue)] public int ClienteId { get; set; }
}
