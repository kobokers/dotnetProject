using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project.Models;

namespace project.Controllers;

[Authorize]
public class FriendController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public FriendController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Requests(string tab = "received")
    {
        ViewBag.Tab = tab;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var friends = await _context.FriendRequests
            .Where(f => f.Status == FriendRequestStatus.Accepted &&
                (f.SenderId == user!.Id || f.ReceiverId == user!.Id))
            .Include(f => f.Sender)
            .Include(f => f.Receiver)
            .ToListAsync();

        var pendingRequests = await _context.FriendRequests
            .Where(f => f.ReceiverId == user!.Id && f.Status == FriendRequestStatus.Pending)
            .Include(f => f.Sender)
            .ToListAsync();

        ViewBag.PendingRequests = pendingRequests;
        return View(friends);
    }

    [HttpPost]
    public async Task<IActionResult> SendRequest(string receiverId)
    {
        var user = await _userManager.GetUserAsync(User);

        var existing = await _context.FriendRequests
            .FirstOrDefaultAsync(f =>
                (f.SenderId == user!.Id && f.ReceiverId == receiverId) ||
                (f.SenderId == receiverId && f.ReceiverId == user!.Id));

        if (existing != null)
        {
            TempData["Error"] = "Friend request already exists.";
            return RedirectToAction("Index", "Home");
        }

        var request = new FriendRequest
        {
            SenderId = user!.Id,
            ReceiverId = receiverId,
            Status = FriendRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.FriendRequests.Add(request);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Friend request sent.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var request = await _context.FriendRequests.FindAsync(id);

        if (request == null || request.ReceiverId != user!.Id) return NotFound();

        request.Status = FriendRequestStatus.Accepted;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var request = await _context.FriendRequests.FindAsync(id);

        if (request == null || request.ReceiverId != user!.Id) return NotFound();

        request.Status = FriendRequestStatus.Rejected;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string friendId)
    {
        var user = await _userManager.GetUserAsync(User);

        var friendship = await _context.FriendRequests
            .FirstOrDefaultAsync(f =>
                f.Status == FriendRequestStatus.Accepted &&
                ((f.SenderId == user!.Id && f.ReceiverId == friendId) ||
                 (f.SenderId == friendId && f.ReceiverId == user!.Id)));

        if (friendship == null) return NotFound();

        _context.FriendRequests.Remove(friendship);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}
