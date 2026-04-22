using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProducto.Data;
using WebApiProducto.Models;
using WebApiProducto.DTOs;

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

    // GET: api/productos?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductoReadDto>>> Get(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10)
    {
        // Validación Senior de parámetros
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1 || pageSize > 50) pageSize = 10;

        var totalRecords = await _context.Productos.CountAsync();
        
        var productos = await _context.Productos
            .OrderBy(p => p.Id) // Importante ordenar siempre antes de paginar
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var dataDto = productos.Select(p => new ProductoReadDto 
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            FechaDeAlta = p.FechaDeAlta,
            Activo = p.Activo
        });

        return Ok(new PagedResponse<ProductoReadDto>(dataDto, pageNumber, pageSize, totalRecords));
    }

    // GET: api/productos/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoReadDto>> Get(int id)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p == null) return NotFound("Producto no encontrado");

        return Ok(new ProductoReadDto 
        {
            Id = p.Id,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Precio = p.Precio,
            FechaDeAlta = p.FechaDeAlta,
            Activo = p.Activo
        });
    }

    // POST: api/productos
    [HttpPost]
    public async Task<ActionResult<ProductoReadDto>> Post([FromBody] ProductoCreateDto dto)
    {
        // Convertimos de DTO de entrada a Modelo de Base de Datos
        var producto = new Producto 
        {
            Nombre = dto.Nombre,
            Descripcion = dto.Descripcion,
            Precio = dto.Precio,
            Activo = dto.Activo,
            FechaDeAlta = DateTime.UtcNow // Valor interno del sistema
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();

        // Devolvemos el objeto creado en formato ReadDto
        return CreatedAtAction(nameof(Get), new { id = producto.Id }, new ProductoReadDto 
        {
            Id = producto.Id,
            Nombre = producto.Nombre,
            Descripcion = producto.Descripcion,
            Precio = producto.Precio,
            FechaDeAlta = producto.FechaDeAlta,
            Activo = producto.Activo
        });
    }

    // PUT: api/productos/5
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> Put(int id, [FromBody] ProductoCreateDto dto)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p == null) return NotFound("Producto no encontrado");
        
        p.Nombre = dto.Nombre;
        p.Precio = dto.Precio;
        p.Descripcion = dto.Descripcion;
        p.Activo = dto.Activo;
        
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
