using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

[Authorize]
public class BookmarkController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookmarkController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var bookmarks = await _context.Bookmarks
            .Where(b => b.UserId == userId)
            .Include(b => b.Post)
                .ThenInclude(p => p.User)
            .Include(b => b.Post.PostImages)
            .Include(b => b.Post.Likes)
            .Include(b => b.Post.Comments)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();

        return View(bookmarks);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleBookmark(int postId)
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, error = "Not authenticated" });
        }

        var existing = await _context.Bookmarks
            .FirstOrDefaultAsync(b => b.UserId == userId && b.PostId == postId);

        if (existing != null)
        {
            _context.Bookmarks.Remove(existing);
            await _context.SaveChangesAsync();
            return Json(new { success = true, action = "removed", postId });
        }
        else
        {
            _context.Bookmarks.Add(new Bookmark { UserId = userId, PostId = postId });
            await _context.SaveChangesAsync();
            return Json(new { success = true, action = "added", postId });
        }
    }
}