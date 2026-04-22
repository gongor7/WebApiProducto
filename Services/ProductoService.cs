using Microsoft.EntityFrameworkCore;
using WebApiProducto.Data;
using WebApiProducto.DTOs;
using WebApiProducto.Models;

namespace WebApiProducto.Services;

public class ProductoService : IProductoService
{
    private readonly ApplicationDbContext _context;

    public ProductoService(ApplicationDbContext context) => _context = context;

    public async Task<PagedResponse<ProductoReadDto>> GetProductosAsync(int pageNumber, int pageSize, string? nombre, string? descripcion)
    {
        var query = _context.Productos.Include(p => p.Categoria).AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(nombre)) 
            query = query.Where(p => p.Nombre.Contains(nombre));
        
        if (!string.IsNullOrWhiteSpace(descripcion)) 
            query = query.Where(p => p.Descripcion.Contains(descripcion));

        var totalRecords = await query.CountAsync();
        var productos = await query.OrderBy(p => p.Id).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        
        var dataDto = productos.Select(p => new ProductoReadDto {
            Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion, Precio = p.Precio,
            CategoriaNombre = p.Categoria?.Nombre ?? "Sin Categoría", FechaDeAlta = p.FechaDeAlta, Activo = p.Activo
        });

        return new PagedResponse<ProductoReadDto>(dataDto, pageNumber, pageSize, totalRecords);
    }

    public async Task<ProductoReadDto?> GetByIdAsync(int id)
    {
        var p = await _context.Productos.Include(p => p.Categoria).FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return null;
        return new ProductoReadDto { Id = p.Id, Nombre = p.Nombre, Descripcion = p.Descripcion, Precio = p.Precio, CategoriaNombre = p.Categoria?.Nombre ?? "Sin Categoría", FechaDeAlta = p.FechaDeAlta, Activo = p.Activo };
    }

    public async Task<ProductoReadDto> CreateAsync(ProductoCreateDto dto)
    {
        var producto = new Producto { Nombre = dto.Nombre, Descripcion = dto.Descripcion, Precio = dto.Precio, CategoriaId = dto.CategoriaId, Activo = dto.Activo, FechaDeAlta = DateTime.UtcNow };
        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        await _context.Entry(producto).Reference(x => x.Categoria).LoadAsync();

        return new ProductoReadDto { Id = producto.Id, Nombre = producto.Nombre, Descripcion = producto.Descripcion, Precio = producto.Precio, CategoriaNombre = producto.Categoria?.Nombre ?? "Sin Categoría", FechaDeAlta = producto.FechaDeAlta, Activo = producto.Activo };
    }

    public async Task<bool> UpdateAsync(int id, ProductoCreateDto dto)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p == null) return false;
        p.Nombre = dto.Nombre; p.Precio = dto.Precio; p.Descripcion = dto.Descripcion; p.CategoriaId = dto.CategoriaId; p.Activo = dto.Activo;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var p = await _context.Productos.FindAsync(id);
        if (p == null) return false;
        _context.Productos.Remove(p);
        await _context.SaveChangesAsync();
        return true;
    }
}
