namespace AecciWeb.DTOs
{
    public class AnnouncementReadDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsUp { get; set; }
        public DateTime? PublishedDate {get; set;}
    }
}
