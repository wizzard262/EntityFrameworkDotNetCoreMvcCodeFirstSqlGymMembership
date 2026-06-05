using GymMembers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymMembers.Controllers;

// Single MVC page that shows members and their classes—no write operations.

public class HomeController(ApplicationDbContext db) : Controller
{
    private readonly ApplicationDbContext _db = db;

    public async Task<IActionResult> Index()
    {
        return View();
    }

    // Free-Tier Azure DBs can go to sleep after inactivity, so we have this endpoint to wake it up.
    public async Task<IActionResult> WakeDatabase([FromServices] ApplicationDbContext _db)
    {
        // A tiny, fast query that wakes the DB
        await _db.Database.ExecuteSqlRawAsync("SELECT 1");
        return Ok("Database is awake");
    }


    // This endpoint is called via AJAX to refresh the members table without reloading the whole page.
    public async Task<IActionResult> GetMembers()
    {
        var members = await _db.GymMembers
            .Include(m => m.ClassSessions)
            .OrderBy(m => m.LastName)
            .ToListAsync();

        return PartialView("_MembersTable", members);
    }

}
