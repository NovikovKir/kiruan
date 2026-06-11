using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

public class MoviesController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public MoviesController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _db.Movies
            .Include(item => item.AudioTracks)
            .Include(item => item.Subtitles)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        ViewData["RequiresAgeConfirmation"] = RequiresAgeConfirmation(movie.AgeRating);
        return View(movie);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ConfirmAge(int id)
    {
        HttpContext.Session.SetString(GetAgeSessionKey(id), "true");
        return RedirectToAction(nameof(Watch), new { id });
    }

    [Authorize]
    public async Task<IActionResult> Watch(int id)
    {
        var movie = await _db.Movies
            .Include(item => item.AudioTracks)
            .Include(item => item.Subtitles)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        if (!await HasActiveSubscriptionAsync())
        {
            TempData["Error"] = "Для просмотра фильма требуется активная подписка.";
            return RedirectToAction("Purchase", "Subscription");
        }

        if (RequiresAgeConfirmation(movie.AgeRating) && !IsAgeConfirmed(id))
        {
            TempData["Error"] = "Для просмотра фильма необходимо подтвердить возраст на странице фильма.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return View(movie);
    }

    private static bool RequiresAgeConfirmation(string ageRating)
    {
        if (string.IsNullOrWhiteSpace(ageRating))
        {
            return false;
        }

        var digits = new string(ageRating.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var age) && age >= 18;
    }

    private bool IsAgeConfirmed(int movieId) =>
        HttpContext.Session.GetString(GetAgeSessionKey(movieId)) == "true";

    private static string GetAgeSessionKey(int movieId) => $"AgeConfirmed_{movieId}";

    private async Task<bool> HasActiveSubscriptionAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return false;
        }

        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return true;
        }

        if (user.SubscriptionEndAt == null)
        {
            return false;
        }

        return string.Equals(user.SubscriptionStatus, "Active", StringComparison.OrdinalIgnoreCase)
            && user.SubscriptionEndAt > DateTime.UtcNow;
    }
}
