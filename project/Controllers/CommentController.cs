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
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        if (string.IsNullOrWhiteSpace(content))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Comment cannot be empty." });
            TempData["Error"] = "Comment cannot be empty.";
            return RedirectToAction("Index", "Post");
        }

        var post = await _context.Posts.FindAsync(postId);
        if (post == null) return NotFound();

        var comment = new Comment
        {
            PostId = postId,
            UserId = user.Id,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        if (post.UserId != user.Id)
        {
            await NotificationController.CreateAsync(_context, post.UserId, NotificationType.Comment, user.Id, postId, $"{user.DisplayName} commented on your post");
        }

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new
            {
                success = true,
                commentId = comment.CommentId,
                content = comment.Content,
                createdAt = comment.CreatedAt.ToString("MMM dd, HH:mm"),
                userDisplayName = user.DisplayName,
                userProfilePhoto = user.ProfilePhoto,
                userId = user.Id,
                postId
            });
        }

        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer)) return RedirectToAction("Index", "Post");
        var openParam = referer.Contains('?') ? '&' : '?';
        return Redirect($"{referer}{openParam}openComments={postId}");
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
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Comment not found." });
            return NotFound();
        }

        if (comment.UserId != user.Id)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Unauthorized." });
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Content cannot be empty." });
            ModelState.AddModelError("", "Content is required.");
            return View(comment);
        }

        comment.Content = content;
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, content, commentId = comment.CommentId });

        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer)) return RedirectToAction("Index", "Post");
        var openParam = referer.Contains('?') ? '&' : '?';
        return Redirect($"{referer}{openParam}openComments={comment.PostId}");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var comment = await _context.Comments.FindAsync(id);
        if (comment == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Comment not found." });
            return NotFound();
        }

        if (comment.UserId != user.Id)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Unauthorized." });
            return Forbid();
        }

        var postId = comment.PostId;
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true, postId });

        var referer = Request.Headers["Referer"].ToString();
        if (string.IsNullOrEmpty(referer)) return RedirectToAction("Index", "Post");
        var openParam = referer.Contains('?') ? '&' : '?';
        return Redirect($"{referer}{openParam}openComments={postId}");
    }
}
