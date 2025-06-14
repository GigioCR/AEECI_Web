// Controllers/ProductsController.cs
using AecciWeb.DTOs;
using AecciWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AecciWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProducts()
        {
            try
            {
                var products = await _context.Products
                                             .AsNoTracking()
                                             .ToListAsync();

                var result = products
                    .Select(p => new ProductReadDto
                    {
                        Id          = p.Id,
                        Name        = p.Name,
                        Description = p.Description,
                        Price       = p.Price,
                        Quantity    = p.Quantity,
                        ImageUrl    = p.ImageUrl
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al obtener productos: {ex.Message}");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductReadDto>> GetProductById(int id)
        {
            var p = await _context.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (p == null)
                return NotFound(new { Message = $"Producto Id={id} no encontrado." });

            return Ok(new ProductReadDto
            {
                Id          = p.Id,
                Name        = p.Name,
                Description = p.Description,
                Price       = p.Price,
                Quantity    = p.Quantity,
                ImageUrl    = p.ImageUrl
            });
        }

        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var imageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl)
                ? "https://placehold.co/300x200/E0E0E0/000000?text=No+Image"
                : dto.ImageUrl!;

            var newProduct = new Product
            {
                Name        = dto.Name,
                Description = dto.Description,
                Price       = dto.Price,
                Quantity    = dto.Quantity,
                ImageUrl    = imageUrl
            };

            try
            {
                await _context.Products.AddAsync(newProduct);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al crear producto: {ex.Message}");
            }

            var readDto = new ProductReadDto
            {
                Id          = newProduct.Id,
                Name        = newProduct.Name,
                Description = newProduct.Description,
                Price       = newProduct.Price,
                Quantity    = newProduct.Quantity,
                ImageUrl    = newProduct.ImageUrl
            };

            return CreatedAtAction(nameof(GetProductById),
                                   new { id = newProduct.Id },
                                   readDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = $"Producto Id={id} no encontrado." });

            existing.Name        = dto.Name;
            existing.Description = dto.Description;
            existing.Price       = dto.Price;
            existing.Quantity    = dto.Quantity;
            existing.ImageUrl    = string.IsNullOrWhiteSpace(dto.ImageUrl)
                                   ? existing.ImageUrl
                                   : dto.ImageUrl!;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar producto: {ex.Message}");
            }

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null)
                return NotFound(new { Message = $"Producto Id={id} no encontrado." });

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
