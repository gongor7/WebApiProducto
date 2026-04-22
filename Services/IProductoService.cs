using WebApiProducto.DTOs;

namespace WebApiProducto.Services;

public interface IProductoService
{
    Task<PagedResponse<ProductoReadDto>> GetProductosAsync(int pageNumber, int pageSize, string? nombre);
    Task<ProductoReadDto?> GetByIdAsync(int id);
    Task<ProductoReadDto> CreateAsync(ProductoCreateDto dto);
    Task<bool> UpdateAsync(int id, ProductoCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
