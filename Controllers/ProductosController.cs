using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProducto.Data;
using WebApiProducto.Models;

namespace WebApiProducto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProductosController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/productos
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> Get()
    {
        return await _context.Productos.ToListAsync();
    }

    // GET: api/productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> Get(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound("Producto no encontrado");
        return producto;
    }

    // POST: api/productos
    [HttpPost]
    public async Task<ActionResult<string>> Post([FromBody] Producto producto)
    {
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return Ok("creado");
    }

    // PUT: api/productos/5
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> Put(int id, [FromBody] Producto producto)
    {
        var existing = await _context.Productos.FindAsync(id);
        if (existing == null) return NotFound("Producto no encontrado");
        
        existing.Nombre = producto.Nombre;
        existing.Precio = producto.Precio;
        existing.Descripcion = producto.Descripcion;
        existing.Activo = producto.Activo;
        
        await _context.SaveChangesAsync();
        return Ok("actualizado");
    }

    // DELETE: api/productos/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> Delete(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound("Producto no encontrado");
        
        _context.Productos.Remove(producto);
        await _context.SaveChangesAsync();
        return Ok("borrado");
    }
}
