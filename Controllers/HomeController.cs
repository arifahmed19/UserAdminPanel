using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAdminPanel.Data;
using UserAdminPanel.Models;

namespace UserAdminPanel.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        var users = _db.Users.OrderByDescending(u => u.LastLoginTime).ToList();
        return View(users);
    }

    [HttpPost]
    public async Task<IActionResult> BulkAction(List<int> selectedUserIds, string actionType)
    {
        if (selectedUserIds == null || !selectedUserIds.Any())
        {
            TempData["ErrorMessage"] = "No users selected.";
            return RedirectToAction("Index");
        }

        var usersToModify = _db.Users.Where(u => selectedUserIds.Contains(u.Id)).ToList();

        foreach (var user in usersToModify)
        {
            if (actionType == "block")
            {
                user.Status = UserStatus.Blocked;
            }
            else if (actionType == "unblock")
            {
                user.Status = UserStatus.Active;
            }
            else if (actionType == "delete")
            {
                _db.Users.Remove(user);
            }
        }

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Operation '{actionType}' completed successfully for {usersToModify.Count} user(s).";
        return RedirectToAction("Index");
    }

    [AllowAnonymous]
    public IActionResult HowTo()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
