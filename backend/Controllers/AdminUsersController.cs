using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Users")]
public class AdminUsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminUsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.OrderBy(user => user.Email).ToListAsync();
        return View(users);
    }

    [HttpGet("Edit/{id}")]
    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var model = new AdminUserEditViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            Role = user.Role,
            SubscriptionStatus = user.SubscriptionStatus,
            SubscriptionEndAt = user.SubscriptionEndAt
        };

        return View(model);
    }

    [HttpPost("Edit/{id}")]
    public async Task<IActionResult> Edit(string id, AdminUserEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = model.Email;
        user.UserName = model.Email;
        user.Role = model.Role;
        user.SubscriptionStatus = model.SubscriptionStatus;
        user.SubscriptionEndAt = model.SubscriptionEndAt;

        var currentRoles = await _userManager.GetRolesAsync(user);
        var targetRole = model.Role == "Admin" ? "Admin" : "User";
        if (!currentRoles.Contains(targetRole))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, targetRole);
        }

        await _userManager.UpdateAsync(user);
        TempData["Success"] = "Пользователь обновлён.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _userManager.DeleteAsync(user);
        TempData["Success"] = "Пользователь удалён.";
        return RedirectToAction(nameof(Index));
    }
}
