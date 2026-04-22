using Microsoft.AspNetCore.Mvc;
using WebApiProducto.Models;

namespace WebApiProducto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    // Lista estática en memoria para simular la base de datos
    private static List<Producto> _productos = new List<Producto>();

    // GET: api/productos
    [HttpGet]
    public ActionResult<IEnumerable<Producto>> Get()
    {
        return Ok(_productos);
    }

    // GET: api/productos/5
    [HttpGet("{id}")]
    public ActionResult<Producto> Get(int id)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound("Producto no encontrado");
        return Ok(producto);
    }

    // POST: api/productos
    [HttpPost]
    public ActionResult<string> Post([FromBody] Producto producto)
    {
        _productos.Add(producto);
        return Ok("creado");
    }

    // PUT: api/productos/5
    [HttpPut("{id}")]
    public ActionResult<string> Put(int id, [FromBody] Producto producto)
    {
        var existing = _productos.FirstOrDefault(p => p.Id == id);
        if (existing == null) return NotFound("Producto no encontrado");
        
        existing.Nombre = producto.Nombre;
        existing.Precio = producto.Precio;
        existing.Descripcion = producto.Descripcion;
        existing.Activo = producto.Activo;
        
        return Ok("actualizado");
    }

    // DELETE: api/productos/5
    [HttpDelete("{id}")]
    public ActionResult<string> Delete(int id)
    {
        var producto = _productos.FirstOrDefault(p => p.Id == id);
        if (producto == null) return NotFound("Producto no encontrado");
        
        _productos.Remove(producto);
        return Ok("borrado");
    }
}
