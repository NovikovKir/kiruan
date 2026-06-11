using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize(Roles = "Admin")]
[Route("Admin/AudioTracks")]
public class AdminAudioTracksController : Controller
{
    private readonly AppDbContext _db;

    public AdminAudioTracksController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(int? movieId)
    {
        var tracks = _db.AudioTracks.Include(track => track.Movie).AsQueryable();
        if (movieId.HasValue)
        {
            tracks = tracks.Where(track => track.MovieId == movieId.Value);
        }

        ViewData["MovieId"] = movieId;
        return View(await tracks.ToListAsync());
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(int? movieId)
    {
        ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
        return View(new AudioTrack { MovieId = movieId ?? 0 });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(AudioTrack track)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
            return View(track);
        }

        _db.AudioTracks.Add(track);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Озвучка добавлена.";
        return RedirectToAction(nameof(Index), new { movieId = track.MovieId });
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var track = await _db.AudioTracks.FindAsync(id);
        if (track == null)
        {
            return NotFound();
        }

        ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
        return View(track);
    }

    [HttpPost("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, AudioTrack track)
    {
        if (id != track.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Movies"] = await _db.Movies.OrderBy(movie => movie.Title).ToListAsync();
            return View(track);
        }

        _db.AudioTracks.Update(track);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Озвучка обновлена.";
        return RedirectToAction(nameof(Index), new { movieId = track.MovieId });
    }

    [HttpPost("Delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var track = await _db.AudioTracks.FindAsync(id);
        if (track == null)
        {
            return NotFound();
        }

        _db.AudioTracks.Remove(track);
        await _db.SaveChangesAsync();
        TempData["Success"] = "Озвучка удалена.";
        return RedirectToAction(nameof(Index), new { movieId = track.MovieId });
    }
}
