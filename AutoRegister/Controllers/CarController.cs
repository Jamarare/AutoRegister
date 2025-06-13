using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CarController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CarController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var cars = isAdmin
            ? await _context.Cars.Include(c => c.Owner).ToListAsync()
            : await _context.Cars.Where(c => c.OwnerId == user.Id).ToListAsync();

        return View(cars);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Car car)
    {
        var user = await _userManager.GetUserAsync(User);
        car.OwnerId = user.Id;

        if (ModelState.IsValid)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(car);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}