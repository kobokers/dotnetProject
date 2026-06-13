using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Helpers;
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
            .Include(p => p.PostImages)
            .AsSplitQuery()
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

            var post = await _context.Posts.FindAsync(postId);
            if (post != null && post.UserId != user.Id)
            {
                await NotificationController.CreateAsync(_context, post.UserId, NotificationType.Like, user.Id, postId, $"{user.DisplayName} liked your post");
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> ModalContent(int id)
    {
        var user = User.Identity?.IsAuthenticated == true ? await _userManager.GetUserAsync(User) : null;
        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.PostImages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null) return NotFound();

        ViewBag.CurrentUserId = user?.Id;
        ViewBag.IsLiked = user != null && post.Likes.Any(l => l.UserId == user.Id);
        return PartialView("_PostModal", post);
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
    [RequestSizeLimit(104_857_600)]
    public async Task<IActionResult> Create(Post model, IFormFile? image, IFormFile[]? images)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var hasContent = !string.IsNullOrWhiteSpace(model.Content);
        var hasImage = (image != null && image.Length > 0) || (images != null && images.Any(f => f.Length > 0));

        if (!hasContent && !hasImage)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Post content or image is required." });
            ModelState.AddModelError("", "Post content or image is required.");
            return View(model);
        }

        model.UserId = user.Id;
        model.CreatedAt = DateTime.UtcNow;
        model.Content ??= string.Empty;

        var uploadedImages = new List<string>();
        var allFiles = new List<IFormFile>();
        if (image != null && image.Length > 0) allFiles.Add(image);
        if (images != null) allFiles.AddRange(images.Where(f => f.Length > 0));

        for (int i = 0; i < allFiles.Count; i++)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(allFiles[i].FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await allFiles[i].CopyToAsync(stream);
            }

            var url = $"/uploads/posts/{fileName}";
            uploadedImages.Add(url);

            if (i == 0 && allFiles.Count == 1 && image != null && image == allFiles[i])
                model.ImageUrl = url;
        }

        if (uploadedImages.Count > 0 && model.ImageUrl == null)
            model.ImageUrl = uploadedImages[0];

        foreach (var url in uploadedImages)
        {
            model.PostImages.Add(new PostImage
            {
                ImageUrl = url,
                Order = model.PostImages.Count
            });
        }

        _context.Posts.Add(model);
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.PostImages)
                .AsSplitQuery()
                .FirstAsync(p => p.PostId == model.PostId);

            var html = await this.RenderPartialToStringAsync("_PostCard", post);

            return Json(new
            {
                success = true,
                html
            });
        }

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.PostId == id);
        if (post == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id) return Forbid();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return PartialView("_EditPostModal", post);

        return RedirectToAction("Index");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(int id, Post model, IFormFile? image, IFormFile[]? images)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.PostId == id);
        if (post == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Post not found." });
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Unauthorized." });
            return Forbid();
        }

        if (string.IsNullOrWhiteSpace(model.Content))
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Content is required." });
            ModelState.AddModelError("Content", "Content is required.");
            model.PostId = id;
            return View(model);
        }

        post.Content = model.Content;
        post.UpdatedAt = DateTime.UtcNow;

        var allFiles = new List<IFormFile>();
        if (image != null && image.Length > 0) allFiles.Add(image);
        if (images != null) allFiles.AddRange(images.Where(f => f.Length > 0));

        foreach (var file in allFiles)
        {
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "posts", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            post.PostImages.Add(new PostImage
            {
                ImageUrl = $"/uploads/posts/{fileName}",
                Order = post.PostImages.Count
            });
        }

        if (post.PostImages.Count > 0)
            post.ImageUrl = post.PostImages.OrderBy(pi => pi.Order).First().ImageUrl;
        else
            post.ImageUrl = null;

        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var updatedPost = await _context.Posts
                .Include(p => p.User)
                .Include(p => p.PostImages)
                .AsSplitQuery()
                .FirstAsync(p => p.PostId == post.PostId);

            var html = await this.RenderPartialToStringAsync("_PostCard", updatedPost);

            return Json(new
            {
                success = true,
                html
            });
        }

        return RedirectToAction("Details", new { id });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var image = await _context.PostImages
            .Include(pi => pi.Post)
            .FirstOrDefaultAsync(pi => pi.PostImageId == imageId);
        if (image == null)
            return Json(new { success = false, error = "Image not found." });

        var user = await _userManager.GetUserAsync(User);
        if (image.Post.UserId != user!.Id)
            return Json(new { success = false, error = "Unauthorized." });

        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(), "wwwroot",
            image.ImageUrl.TrimStart('/'));
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.PostImages.Remove(image);
        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _context.Posts
            .Include(p => p.PostImages)
            .FirstOrDefaultAsync(p => p.PostId == id);
        if (post == null)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Post not found." });
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (post.UserId != user!.Id)
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = false, error = "Unauthorized." });
            return Forbid();
        }

        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        foreach (var pi in post.PostImages)
        {
            var filePath = Path.Combine(uploadsDir, pi.ImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);
        }

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = true });

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> LightboxData(int id)
    {
        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.PostImages)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null) return Json(new { success = false });

        var user = User.Identity?.IsAuthenticated == true ? await _userManager.GetUserAsync(User) : null;
        var isLiked = user != null && post.Likes.Any(l => l.UserId == user.Id);

        return Json(new
        {
            success = true,
            postId = post.PostId,
            content = post.Content ?? "",
            authorName = post.User.DisplayName,
            authorPhoto = post.User.ProfilePhoto ?? "",
            authorId = post.UserId,
            createdAt = post.CreatedAt.ToString("MMM dd, yyyy HH:mm"),
            likeCount = post.Likes.Count,
            commentCount = post.Comments.Count,
            isLiked = isLiked,
            images = post.PostImages.OrderBy(pi => pi.Order).Select(pi => pi.ImageUrl).ToList(),
            comments = post.Comments.OrderBy(c => c.CreatedAt).Select(c => new
            {
                commentId = c.CommentId,
                content = c.Content,
                authorName = c.User.DisplayName,
                authorPhoto = c.User.ProfilePhoto ?? "",
                authorId = c.UserId,
                createdAt = c.CreatedAt.ToString("MMM dd, HH:mm")
            }).ToList()
        });
    }
}
