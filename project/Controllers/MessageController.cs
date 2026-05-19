using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

[Authorize]
public class MessageController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MessageController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var userId = user!.Id;

        // Get accepted friends (both sent and received)
        var friendIds = await _context.FriendRequests
            .Where(f => f.Status == FriendRequestStatus.Accepted)
            .Where(f => f.SenderId == userId || f.ReceiverId == userId)
            .Select(f => f.SenderId == userId ? f.ReceiverId : f.SenderId)
            .Distinct()
            .ToListAsync();

        var friends = await _context.Users
            .Where(u => friendIds.Contains(u.Id))
            .ToListAsync();

        // Get last message for each conversation
        var conversations = new List<(ApplicationUser Friend, Message? LastMessage)>();
        foreach (var friend in friends)
        {
            var lastMessage = await _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == friend.Id)
                         || (m.SenderId == friend.Id && m.ReceiverId == userId))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            conversations.Add((friend, lastMessage));
        }

        // Sort by most recent message first
        conversations = conversations
            .OrderByDescending(c => c.LastMessage?.CreatedAt)
            .ToList();

        return View(conversations);
    }

    [HttpGet]
    public async Task<IActionResult> Conversation(string userId)
    {
        var user = await _userManager.GetUserAsync(User);
        var currentUserId = user!.Id;

        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Index");

        // Verify they are friends
        var areFriends = await _context.FriendRequests
            .AnyAsync(f => f.Status == FriendRequestStatus.Accepted
                && ((f.SenderId == currentUserId && f.ReceiverId == userId)
                 || (f.SenderId == userId && f.ReceiverId == currentUserId)));

        if (!areFriends) return Forbid();

        var otherUser = await _context.Users.FindAsync(userId);
        if (otherUser == null) return NotFound();

        // Get messages between the two users
        var messages = await _context.Messages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == userId)
                     || (m.SenderId == userId && m.ReceiverId == currentUserId))
            .Include(m => m.Sender)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();

        // Mark received messages as read
        var unread = messages.Where(m => m.ReceiverId == currentUserId && !m.IsRead).ToList();
        foreach (var msg in unread)
        {
            msg.IsRead = true;
        }
        await _context.SaveChangesAsync();

        // Also get the friend list for the left sidebar
        var friendIds = await _context.FriendRequests
            .Where(f => f.Status == FriendRequestStatus.Accepted)
            .Where(f => f.SenderId == currentUserId || f.ReceiverId == currentUserId)
            .Select(f => f.SenderId == currentUserId ? f.ReceiverId : f.SenderId)
            .Distinct()
            .ToListAsync();

        var friends = await _context.Users
            .Where(u => friendIds.Contains(u.Id))
            .ToListAsync();

        var conversations = new List<(ApplicationUser Friend, Message? LastMessage)>();
        foreach (var friend in friends)
        {
            var lastMessage = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == friend.Id)
                         || (m.SenderId == friend.Id && m.ReceiverId == currentUserId))
                .OrderByDescending(m => m.CreatedAt)
                .FirstOrDefaultAsync();

            conversations.Add((friend, lastMessage));
        }

        conversations = conversations
            .OrderByDescending(c => c.LastMessage?.CreatedAt)
            .ToList();

        ViewBag.OtherUser = otherUser;
        ViewBag.Conversations = conversations;

        return View(messages);
    }

    [HttpPost]
    public async Task<IActionResult> Send(string receiverId, string content)
    {
        if (string.IsNullOrWhiteSpace(content)) return RedirectToAction("Conversation", new { userId = receiverId });

        var user = await _userManager.GetUserAsync(User);
        var currentUserId = user!.Id;

        // Verify they are friends
        var areFriends = await _context.FriendRequests
            .AnyAsync(f => f.Status == FriendRequestStatus.Accepted
                && ((f.SenderId == currentUserId && f.ReceiverId == receiverId)
                 || (f.SenderId == receiverId && f.ReceiverId == currentUserId)));

        if (!areFriends) return Forbid();

        var message = new Message
        {
            SenderId = currentUserId,
            ReceiverId = receiverId,
            Content = content,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        await NotificationController.CreateAsync(_context, receiverId, NotificationType.Message, currentUserId, message.MessageId, $"{user.DisplayName} sent you a message");

        return RedirectToAction("Conversation", new { userId = receiverId });
    }
}
