using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;
using System;
using System.Linq;
using System.Collections.Generic;

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
    public async Task<IActionResult> Create(IFormFile? image, IFormFile[]? images, string? content, string? backgroundColor, string? fontStyle)
    {
        var user = await _userManager.GetUserAsync(User);

        // Validate that at least one image/video or text is provided
        var hasImage = (image != null && image.Length > 0) || (images != null && images.Any(i => i?.Length > 0));
        if (!hasImage && string.IsNullOrWhiteSpace(content))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Please select an image/video or enter text." });
            ModelState.AddModelError("", "Please select an image/video or enter text.");
            return View();
        }

        // Enforce max 8 files
        if (images != null && images.Length > 8)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Maximum 8 images allowed." });
            ModelState.AddModelError("", "Maximum 8 images allowed.");
            return View();
        }

        // Enforce per‑file size limit (10 MiB)
        var allFiles = new List<IFormFile>();
        if (image != null && image.Length > 0) allFiles.Add(image);
        if (images != null) allFiles.AddRange(images.Where(i => i?.Length > 0));
        if (allFiles.Any(f => f.Length > 10_485_760))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "File exceeds 10 MiB limit." });
            ModelState.AddModelError("", "File exceeds 10 MiB limit.");
            return View();
        }

        var story = new Story
        {
            UserId = user!.Id,
            CreatedAt = DateTime.UtcNow
        };

        if (allFiles.Any())
        {
            foreach (var file in allFiles)
            {
                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "stories", fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);
                story.StoryImages.Add(new StoryImage
                {
                    ImageUrl = $"/uploads/stories/{fileName}",
                    Order = story.StoryImages.Count
                });
            }
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
                imageUrl = story.StoryImages.FirstOrDefault()?.ImageUrl ?? story.ImageUrl ?? "",
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
        var story = await _context.Stories
            .Include(s => s.StoryImages)
            .FirstOrDefaultAsync(s => s.StoryId == id);
        if (story == null)
            return Json(new { success = false, error = "Story not found." });

        var user = await _userManager.GetUserAsync(User);
        if (story.UserId != user!.Id)
            return Json(new { success = false, error = "Unauthorized." });

        foreach (var image in story.StoryImages)
        {
            if (!string.IsNullOrEmpty(image.ImageUrl))
            {
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot",
                    image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
        }

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
