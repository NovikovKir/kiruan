using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Subtitles")]
public class AdminSubtitlesController : Controller
{
    private readonly AppDbContext _db;

    public AdminSubtitlesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? movieId)
    {
        var subtitles = _db.Subtitles.Include(item => item.Movie).AsQueryable();
        if (movieId.HasValue)
        {
            subtitles = subtitles.Where(item => item.MovieId == movieId.Value);
        }

        ViewData["MovieId"] = movieId;
        return View(await subtitles.ToListAsync());
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(int? movieId)
    {
        ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
        return View(new Subtitle { MovieId = movieId ?? 0 });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Subtitle subtitle)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
            return View(subtitle);
        }

        _db.Subtitles.Add(subtitle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Субтитры добавлены.";
        return RedirectToAction(nameof(Index), new { movieId = subtitle.MovieId });
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var subtitle = await _db.Subtitles.FindAsync(id);
        if (subtitle == null)
        {
            return NotFound();
        }

        ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
        return View(subtitle);
    }

    [HttpPost("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, Subtitle subtitle)
    {
        if (id != subtitle.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
            return View(subtitle);
        }

        _db.Subtitles.Update(subtitle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Субтитры обновлены.";
        return RedirectToAction(nameof(Index), new { movieId = subtitle.MovieId });
    }

    [HttpPost("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var subtitle = await _db.Subtitles.FindAsync(id);
        if (subtitle == null)
        {
            return NotFound();
        }

        _db.Subtitles.Remove(subtitle);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Субтитры удалены.";
        return RedirectToAction(nameof(Index), new { movieId = subtitle.MovieId });
    }
}
