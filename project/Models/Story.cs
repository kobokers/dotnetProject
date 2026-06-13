using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models;

public class Story
{
    public int StoryId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    // Collection of media items (images or MP4 videos) for this story. Legacy ImageUrl remains as a fallback.
    public ICollection<StoryImage> StoryImages { get; set; } = new List<StoryImage>();

    [Column("TextContent")]
    public string? Content { get; set; }

    [Column("Background")]
    public string? BackgroundColor { get; set; }

    public string? FontStyle { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
