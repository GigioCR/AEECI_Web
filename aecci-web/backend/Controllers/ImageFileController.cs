// Controllers/ImageFileController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace AecciWeb.Controllers
{
    public class ImageUploadDto
    {
        public int OwnerId { get; set; }
        public string OwnerType { get; set; } = string.Empty;
        public List<IFormFile>? Images { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ImageFileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public ImageFileController(IWebHostEnvironment env) => _env = env;

        [HttpPost("upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto dto)
        {
            if (dto.Images == null || !dto.Images.Any())
                return BadRequest("No se enviaron imágenes.");

            try
            {
                // si WebRootPath es null, lo apuntamos a ContentRootPath/wwwroot
                var webRoot = _env.WebRootPath;
                if (string.IsNullOrEmpty(webRoot))
                {
                    webRoot = Path.Combine(_env.ContentRootPath, "wwwroot");
                }

                var uploadsDir = Path.Combine(webRoot, "uploads");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var urls = new List<string>();
                foreach (var file in dto.Images)
                {
                    var ext      = Path.GetExtension(file.FileName);
                    var name     = $"{Guid.NewGuid()}{ext}";
                    var fullPath = Path.Combine(uploadsDir, name);

                    using var stream = System.IO.File.Create(fullPath);
                    await file.CopyToAsync(stream);

                    urls.Add($"/uploads/{name}");
                }

                return Ok(new { Urls = urls });
            }
            catch (Exception ex)
            {
                // aquí podrías loguear ex.Message
                return StatusCode(500, $"Error subiendo imágenes: {ex.Message}");
            }
        }
    }
}
