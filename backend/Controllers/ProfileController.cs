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
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            PopulateViewData(user);
            return View("Index", model);
        }

        if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
        {
            var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
            if (!setUserNameResult.Succeeded)
            {
                AddIdentityErrors(setUserNameResult);
                PopulateViewData(user);
                return View("Index", model);
            }

            var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            if (!setEmailResult.Succeeded)
            {
                AddIdentityErrors(setEmailResult);
                PopulateViewData(user);
                return View("Index", model);
            }

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        if (!string.IsNullOrWhiteSpace(model.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(model.CurrentPassword))
            {
                ModelState.AddModelError(string.Empty, "Для смены пароля укажите текущий пароль.");
                PopulateViewData(user);
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

                PopulateViewData(user);
                return View("Index", model);
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        TempData["Success"] = "Данные обновлены.";
        return RedirectToAction("Index");
    }

    private void PopulateViewData(ApplicationUser user)
    {
        ViewData["SubscriptionStatus"] = user.SubscriptionStatus;
        ViewData["SubscriptionEndAt"] = user.SubscriptionEndAt?.ToString("dd.MM.yyyy") ?? "Нет";
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
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
