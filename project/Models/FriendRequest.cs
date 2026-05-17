using System.ComponentModel.DataAnnotations;

namespace project.Models;

public enum FriendRequestStatus
{
    Pending,
    Accepted,
    Rejected
}

public class FriendRequest
{
    public int FriendRequestId { get; set; }

    [Required]
    public string SenderId { get; set; } = string.Empty;

    [Required]
    public string ReceiverId { get; set; } = string.Empty;

    [Required]
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Sender { get; set; } = null!;
    public ApplicationUser Receiver { get; set; } = null!;
}
