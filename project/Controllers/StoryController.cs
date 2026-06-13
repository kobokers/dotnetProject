using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using project.Models;
using System;

namespace project.Controllers;

[Authorize]
public class StoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public StoryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(IFormFile? image, string? content, string? backgroundColor, string? fontStyle)
    {
        var user = await _userManager.GetUserAsync(User);

        if ((image == null || image.Length == 0) && string.IsNullOrWhiteSpace(content))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Please select an image or enter text." });
            ModelState.AddModelError("", "Please select an image or enter text.");
            return View();
        }

        var story = new Story
        {
            UserId = user!.Id,
            CreatedAt = DateTime.UtcNow
        };

        if (image != null && image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "stories", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            story.ImageUrl = $"/uploads/stories/{fileName}";
        }
        else
        {
            story.Content = content;
            story.BackgroundColor = backgroundColor ?? "#5865f2";
            story.FontStyle = fontStyle ?? "sans-serif";
        }

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new
            {
                success = true,
                storyId = story.StoryId,
                imageUrl = story.ImageUrl ?? "",
                content = story.Content ?? "",
                backgroundColor = story.BackgroundColor ?? "",
                fontStyle = story.FontStyle ?? "",
                userId = user.Id,
                userDisplayName = user.DisplayName ?? user.UserName,
                userProfilePhoto = user.ProfilePhoto ?? ""
            });
        }

        return RedirectToAction("Index", "Post");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var story = await _context.Stories.FindAsync(id);
        if (story == null)
            return Json(new { success = false, error = "Story not found." });

        var user = await _userManager.GetUserAsync(User);
        if (story.UserId != user!.Id)
            return Json(new { success = false, error = "Unauthorized." });

        if (!string.IsNullOrEmpty(story.ImageUrl))
        {
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot",
                story.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _context.Stories.Remove(story);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }
}
