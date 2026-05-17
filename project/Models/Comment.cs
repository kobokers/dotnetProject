using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Comment
{
    public int CommentId { get; set; }

    [Required]
    public int PostId { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Comment content is required.")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 2000 characters.")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Post Post { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}
