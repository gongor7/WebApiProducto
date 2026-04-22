using System.ComponentModel.DataAnnotations;

namespace WebApiProducto.Models;

public class Categoria
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    // Relación inversa: Una categoría tiene muchos productos
    public ICollection<Producto> Productos { get; set; } = new List<Producto>();
}
