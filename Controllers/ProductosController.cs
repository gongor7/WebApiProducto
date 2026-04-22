using Microsoft.AspNetCore.Mvc;
using WebApiProducto.DTOs;
using WebApiProducto.Services;

namespace WebApiProducto.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly IProductoService _service;

    public ProductosController(IProductoService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductoReadDto>>> Get(
        [FromQuery] int pageNumber = 1, 
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nombre = null,
        [FromQuery] string? descripcion = null)
        => Ok(await _service.GetProductosAsync(pageNumber, pageSize, nombre, descripcion));

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoReadDto>> Get(int id)
    {
        var p = await _service.GetByIdAsync(id);
        return p != null ? Ok(p) : NotFound("Producto no encontrado");
    }

    [HttpPost]
    public async Task<ActionResult<ProductoReadDto>> Post([FromBody] ProductoCreateDto dto)
        => CreatedAtAction(nameof(Get), new { id = (await _service.CreateAsync(dto)).Id }, await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, [FromBody] ProductoCreateDto dto)
        => await _service.UpdateAsync(id, dto) ? Ok("actualizado") : NotFound("Producto no encontrado");

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? Ok("borrado") : NotFound("Producto no encontrado");
}
