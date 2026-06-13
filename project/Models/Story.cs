using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Story
{
    public int StoryId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string? Content { get; set; }

    public string? BackgroundColor { get; set; }

    public string? FontStyle { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
