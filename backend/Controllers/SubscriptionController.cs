using backend.Data;
using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
public class SubscriptionController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public SubscriptionController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Purchase()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            TempData["Success"] = "Администратору доступна безлимитная подписка.";
            return RedirectToAction("Index", "Profile");
        }

        return View(new SubscriptionPurchaseViewModel { ExpYear = DateTime.UtcNow.Year });
    }

    [HttpPost]
    public async Task<IActionResult> Purchase(SubscriptionPurchaseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var months = model.Plan == "Год" ? 12 : 1;
        var start = DateTime.UtcNow;
        var end = start.AddMonths(months);

        var subscription = new Subscription
        {
            UserId = user.Id,
            Plan = model.Plan,
            StartAt = start,
            EndAt = end,
            Status = "Active"
        };

        _db.Subscriptions.Add(subscription);

        user.SubscriptionStatus = "Active";
        user.SubscriptionEndAt = end;
        await _userManager.UpdateAsync(user);

        await _db.SaveChangesAsync();

        TempData["Success"] = "Подписка успешно оформлена.";
        return RedirectToAction("Index", "Profile");
    }
}
