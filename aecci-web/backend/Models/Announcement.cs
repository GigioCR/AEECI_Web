using System;
using System.ComponentModel.DataAnnotations;

namespace AecciWeb.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Body { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public bool IsUp { get; set; } = true;

        public DateTime PublishedDate{get; set;}
    }
}
