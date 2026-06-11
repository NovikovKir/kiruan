using System.Diagnostics;
using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? query)
    {
        var movies = _db.Movies.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            movies = movies.Where(movie => movie.Title.Contains(query));
        }

        var result = await movies
            .OrderBy(movie => movie.Title)
            .ToListAsync();

        ViewData["Query"] = query ?? string.Empty;
        return View(result);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
