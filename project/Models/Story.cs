using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Story
{
    public int StoryId { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Story image is required.")]
    public string ImageUrl { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
}
