using project.Models;

public class Bookmark
{
    public int BookmarkId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}