using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

[Authorize]
public class CommentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CommentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> Create(int postId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["Error"] = "Comment cannot be empty.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        var user = await _userManager.GetUserAsync(User);
        var post = await _context.Posts.FindAsync(postId);
        if (post == null) return NotFound();

        var comment = new Comment
        {
            PostId = postId,
            UserId = user!.Id,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        if (post.UserId != user.Id)
        {
            await NotificationController.CreateAsync(_context, post.UserId, NotificationType.Comment, user.Id, postId, $"{user.DisplayName} commented on your post");
        }

        return RedirectToAction("Details", "Post", new { id = postId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var comment = await _context.Comments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.CommentId == id);

        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (comment.UserId != user!.Id) return Forbid();

        return View(comment);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string content)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (comment.UserId != user!.Id) return Forbid();

        if (string.IsNullOrWhiteSpace(content))
        {
            ModelState.AddModelError("", "Content is required.");
            return View(comment);
        }

        comment.Content = content;
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Post", new { id = comment.PostId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var comment = await _context.Comments.FindAsync(id);
        if (comment == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (comment.UserId != user!.Id) return Forbid();

        var postId = comment.PostId;
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", "Post", new { id = postId });
    }
}
