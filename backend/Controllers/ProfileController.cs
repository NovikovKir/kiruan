using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ProfileViewModel
        {
            Email = user.Email ?? string.Empty
        };

        ViewData["SubscriptionStatus"] = user.SubscriptionStatus;
        ViewData["SubscriptionEndAt"] = user.SubscriptionEndAt?.ToString("dd.MM.yyyy") ?? "Нет";

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Update(ProfileViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = model.Email;
            user.UserName = model.Email;
        }

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError(string.Empty, "Для смены пароля укажите текущий пароль.");
                return View("Index", model);
            }

            var passwordResult = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword);

            if (!passwordResult.Succeeded)
            {
                foreach (var error in passwordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("Index", model);
            }
        }

        await _userManager.UpdateAsync(user);
        await _signInManager.RefreshSignInAsync(user);
        TempData["Success"] = "Данные обновлены.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        await _signInManager.SignOutAsync();
        await _userManager.DeleteAsync(user);
        TempData["Success"] = "Аккаунт удалён.";
        return RedirectToAction("Index", "Home");
    }
}
