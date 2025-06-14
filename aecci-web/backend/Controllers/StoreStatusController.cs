using AecciWeb.DTOs;
using AecciWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AecciWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreStatusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StoreStatusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/StoreStatus
        [HttpGet]
        public async Task<ActionResult<StoreStatusDto>> GetStatus()
        {
            var status = await _context.StoreStatus.AsNoTracking().OrderByDescending(s => s.Id).FirstOrDefaultAsync();
            if (status == null)
                return NotFound();

            return new StoreStatusDto {
                Id = status.Id,
                IsOpen = status.IsOpen,
            };
        }

        // PUT: api/StoreStatus
        [HttpPut]
        public async Task<IActionResult> UpdateStatus([FromBody] StoreStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _context.StoreStatus.FindAsync(dto.Id);
            if (existing == null)
                return NotFound();

            existing.IsOpen = dto.IsOpen;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}