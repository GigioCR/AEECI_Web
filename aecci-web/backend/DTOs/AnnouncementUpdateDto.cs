namespace AecciWeb.DTOs
{
    public class AnnouncementUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsUp { get; set; }
    }
}
