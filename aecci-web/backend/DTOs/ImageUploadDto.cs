public class ImageUploadDto
{
    public int OwnerId    { get; set; }
    public string OwnerType { get; set; } = string.Empty;
    public List<IFormFile> Images { get; set; } = new();
}