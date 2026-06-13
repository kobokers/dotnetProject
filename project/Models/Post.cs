using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Post
{
    public int PostId { get; set; }

    public string UserId { get; set; } = string.Empty;

    [StringLength(5000, ErrorMessage = "Content must be at most 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public ICollection<PostImage> PostImages { get; set; } = new List<PostImage>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(UserId))]
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    public ApplicationUser? User { get; set; }
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
