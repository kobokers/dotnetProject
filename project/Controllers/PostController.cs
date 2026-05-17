using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

public class PostController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var cutoff = DateTime.UtcNow.AddHours(-24);
        ViewBag.Stories = await _context.Stories
            .Where(s => s.CreatedAt >= cutoff)
            .Include(s => s.User)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return View(posts);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleLike(int postId)
    {
        var user = await _userManager.GetUserAsync(User);
        var existing = await _context.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == user!.Id);

        if (existing != null)
        {
            _context.Likes.Remove(existing);
        }
        else
        {
            _context.Likes.Add(new Like
            {
                PostId = postId,
                UserId = user!.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null) return NotFound();
        return View(post);
    }

    [Authorize]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Post());
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(Post model, IFormFile? image)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        model.UserId = user!.Id;
        model.CreatedAt = DateTime.UtcNow;

        if (image != null && image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            model.ImageUrl = $"/uploads/posts/{fileName}";
        }

        _context.Posts.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id) return Forbid();

        return View(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Post model)
    {
        if (!ModelState.IsValid)
        {
            model.PostId = id;
            return View(model);
        }

        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id) return Forbid();

        post.Content = model.Content;
        post.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RedirectToAction("Details", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.Posts.FindAsync(id);
        if (post == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id) return Forbid();

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
