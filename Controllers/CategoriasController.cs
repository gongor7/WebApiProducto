using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiProducto.Data;
using WebApiProducto.Models;
using WebApiProducto.DTOs;

namespace WebApiProducto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/categorias
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaReadDto>>> Get()
    {
        var categorias = await _context.Categorias.ToListAsync();
        return Ok(categorias.Select(c => new CategoriaReadDto { Id = c.Id, Nombre = c.Nombre }));
    }

    // GET: api/categorias/5
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaReadDto>> Get(int id)
    {
        var c = await _context.Categorias.FindAsync(id);
        if (c == null) return NotFound("Categoría no encontrada");
        return Ok(new CategoriaReadDto { Id = c.Id, Nombre = c.Nombre });
    }

    // POST: api/categorias
    [HttpPost]
    public async Task<ActionResult<CategoriaReadDto>> Post([FromBody] CategoriaCreateDto dto)
    {
        var categoria = new Categoria { Nombre = dto.Nombre };
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(Get), new { id = categoria.Id }, new CategoriaReadDto { Id = categoria.Id, Nombre = categoria.Nombre });
    }

    // PUT: api/categorias/5
    [HttpPut("{id}")]
    public async Task<ActionResult<string>> Put(int id, [FromBody] CategoriaCreateDto dto)
    {
        var c = await _context.Categorias.FindAsync(id);
        if (c == null) return NotFound("Categoría no encontrada");
        
        c.Nombre = dto.Nombre;
        await _context.SaveChangesAsync();
        return Ok("actualizado");
    }

    // DELETE: api/categorias/5
    [HttpDelete("{id}")]
    public async Task<ActionResult<string>> Delete(int id)
    {
        var c = await _context.Categorias.FindAsync(id);
        if (c == null) return NotFound("Categoría no encontrada");
        
        _context.Categorias.Remove(c);
        await _context.SaveChangesAsync();
        return Ok("borrado");
    }
}
