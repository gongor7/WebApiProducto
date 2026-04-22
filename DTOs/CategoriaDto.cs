using System.ComponentModel.DataAnnotations;

namespace WebApiProducto.DTOs;

public class CategoriaCreateDto
{
    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}

public class CategoriaReadDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
}
