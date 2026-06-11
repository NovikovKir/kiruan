using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/Movies")]
public class AdminMoviesController : Controller
{
    private readonly AppDbContext _db;

    public AdminMoviesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        var movies = await _db.Movies
            .OrderBy(movie => movie.Title)
            .ToListAsync();
        return View(movies);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View(new Movie());
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Фильм добавлен.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    [HttpPost("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        _db.Movies.Update(movie);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Фильм обновлён.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Фильм удалён.";
        return RedirectToAction(nameof(Index));
    }
}
