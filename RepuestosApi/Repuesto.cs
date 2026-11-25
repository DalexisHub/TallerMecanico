namespace RepuestosApi;

public record Repuesto(int Id, string Codigo, string Nombre, string Marca,
    string Categoria, decimal Precio, int Stock);
