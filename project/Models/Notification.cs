namespace project.Models;

public enum NotificationType
{
    FriendRequest,
    FriendAccepted,
    Like,
    Comment,
    Message
}

public class Notification
{
    public int NotificationId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public string FromUserId { get; set; } = string.Empty;
    public int? ReferenceId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = null!;
    public ApplicationUser FromUser { get; set; } = null!;
}
