namespace TallerMecanico.Entidades.Dtos;

public class VehiculoDto
{
    public int Id { get; set; }
    public string Placa { get; set; } = null!;
    public string Marca { get; set; } = null!;
    public string Modelo { get; set; } = null!;
    public int Anio { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNombre { get; set; } = null!;
}
