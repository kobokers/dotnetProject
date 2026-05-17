using System.ComponentModel.DataAnnotations;

namespace project.Models;

public class Message
{
    public int MessageId { get; set; }

    [Required]
    public string SenderId { get; set; } = string.Empty;

    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Message cannot be empty.")]
    [StringLength(5000, MinimumLength = 1, ErrorMessage = "Message must be between 1 and 5000 characters.")]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsRead { get; set; }

    public ApplicationUser Sender { get; set; } = null!;
    public ApplicationUser Receiver { get; set; } = null!;
}
