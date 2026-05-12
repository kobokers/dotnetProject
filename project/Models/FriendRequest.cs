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
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Sender { get; set; } = null!;
    public ApplicationUser Receiver { get; set; } = null!;
}
