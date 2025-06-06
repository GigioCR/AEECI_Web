using AecciWeb.DTOs;
using AecciWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AecciWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductReadDto>>> GetAllProducts()
        {
            var products = await _context.Products
                .AsNoTracking()
                .ToListAsync();

            var result = products.Select(p => new ProductReadDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                ImageUrl = p.ImageUrl
            });

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductReadDto>> GetProductById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound(new { Message = $"Producto con Id={id} no encontrado." });
            }

            var readDto = new ProductReadDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                ImageUrl = product.ImageUrl
            };

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductReadDto>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newProduct = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Quantity = dto.Quantity,
                ImageUrl = dto.ImageUrl
            };

            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            var readDto = new ProductReadDto
            {
                Id = newProduct.Id,
                Name = newProduct.Name,
                Description = newProduct.Description,
                Price = newProduct.Price,
                Quantity = newProduct.Quantity,
                ImageUrl = newProduct.ImageUrl
            };

            return CreatedAtAction(nameof(GetProductById), new { id = newProduct.Id }, readDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { Message = $"Producto con Id={id} no encontrado." });
            }

            existingProduct.Name = dto.Name;
            existingProduct.Description = dto.Description;
            existingProduct.Price = dto.Price;
            existingProduct.Quantity = dto.Quantity;
            existingProduct.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { Message = $"Producto con Id={id} no encontrado." });
            }

            _context.Products.Remove(existingProduct);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
