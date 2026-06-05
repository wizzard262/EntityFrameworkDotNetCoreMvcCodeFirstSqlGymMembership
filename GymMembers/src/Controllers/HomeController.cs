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
        var members = await _db.GymMembers
            .Include(m => m.ClassSessions)
            .OrderBy(m => m.LastName)
            .ToListAsync();

        return View(members);
    }
}
