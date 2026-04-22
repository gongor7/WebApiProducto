using System.ComponentModel.DataAnnotations;

namespace WebApiProducto.DTOs;

public class ProductoCreateDto
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 1000000, ErrorMessage = "El precio debe ser un valor positivo.")]
    public decimal Precio { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public int CategoriaId { get; set; }

    public bool Activo { get; set; } = true;
}

public class ProductoReadDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string CategoriaNombre { get; set; } = string.Empty;
    public DateTime FechaDeAlta { get; set; }
    public bool Activo { get; set; }
}
