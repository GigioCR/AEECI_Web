using AecciWeb.DTOs;
using AecciWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AecciWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnnouncementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AnnouncementReadDto>>> GetActiveAnnouncements()
        {
            var announcements = await _context.Announcements
                .AsNoTracking()
                .Where(a => a.IsUp == true)
                .ToListAsync();

            var result = announcements.Select(a => new AnnouncementReadDto
            {
                Id = a.Id,
                Title = a.Title,
                Body = a.Body,
                ImageUrl = a.ImageUrl,
                IsUp = a.IsUp,
                PublishedDate = a.PublishedDate
            });

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnnouncementReadDto>>> GetAllAnnouncements()
        {
            var announcements = await _context.Announcements
                .AsNoTracking()
                .ToListAsync();

            var result = announcements.Select(a => new AnnouncementReadDto
            {
                Id = a.Id,
                Title = a.Title,
                Body = a.Body,
                ImageUrl = a.ImageUrl,
                IsUp = a.IsUp,
                PublishedDate = a.PublishedDate
            });

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AnnouncementReadDto>> GetAnnouncementById(int id)
        {
            var announcement = await _context.Announcements
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound(new { Message = $"Anuncio con Id={id} no encontrado." });
            }

            var readDto = new AnnouncementReadDto
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Body = announcement.Body,
                ImageUrl = announcement.ImageUrl,
                PublishedDate = announcement.PublishedDate
            };

            return Ok(readDto);
        }

        [HttpPost]
        public async Task<ActionResult<AnnouncementReadDto>> CreateAnnouncement([FromBody] AnnouncementCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newAnnouncement = new Announcement
            {
                Title = dto.Title,
                Body = dto.Body,
                ImageUrl = dto.ImageUrl,
                IsUp = true
            };

            await _context.Announcements.AddAsync(newAnnouncement);
            await _context.SaveChangesAsync();

            var readDto = new AnnouncementReadDto
            {
                Id = newAnnouncement.Id,
                Title = newAnnouncement.Title,
                Body = newAnnouncement.Body,
                ImageUrl = newAnnouncement.ImageUrl
            };

            return CreatedAtAction(nameof(GetAnnouncementById), new { id = newAnnouncement.Id }, readDto);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAnnouncement(int id, [FromBody] AnnouncementUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingAnnouncement = await _context.Announcements.FindAsync(id);
            if (existingAnnouncement == null)
            {
                return NotFound(new { Message = $"Anuncio con Id={id} no encontrado." });
            }

            existingAnnouncement.Title = dto.Title;
            existingAnnouncement.Body = dto.Body;
            existingAnnouncement.ImageUrl = dto.ImageUrl;
            existingAnnouncement.IsUp = dto.IsUp;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAnnouncement(int id)
        {
            var existingAnnouncement = await _context.Announcements.FindAsync(id);
            if (existingAnnouncement == null)
            {
                return NotFound(new { Message = $"Announcement con Id={id} no encontrado." });
            }

            _context.Announcements.Remove(existingAnnouncement);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
