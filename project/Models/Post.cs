using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required.")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public ApplicationUser User { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
