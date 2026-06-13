namespace project.Models;

public class PostImage
{
    public int PostImageId { get; set; }
    public int PostId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
    public Post Post { get; set; } = null!;
}
