using Microsoft.AspNetCore.Identity;

namespace project.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePhoto { get; set; }
    public string? CoverPhoto { get; set; }
    public string? Location { get; set; }
    public string FriendCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<FriendRequest> SentRequests { get; set; } = new List<FriendRequest>();
    public ICollection<FriendRequest> ReceivedRequests { get; set; } = new List<FriendRequest>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Story> Stories { get; set; } = new List<Story>();
    public ICollection<Message> SentMessages { get; set; } = new List<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();
}
