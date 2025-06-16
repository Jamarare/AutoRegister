using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

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
    [ValidateAntiForgeryToken]
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

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && car.OwnerId != user.Id)
            return Forbid();

        return View(car);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Car updatedCar)
    {
        if (id != updatedCar.Id) return NotFound();

        if (!ModelState.IsValid) return View(updatedCar);

        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && car.OwnerId != user.Id)
            return Forbid();

        car.Make = updatedCar.Make;
        car.Model = updatedCar.Model;
        car.LicensePlate = updatedCar.LicensePlate;
        car.Year = updatedCar.Year;

        try
        {
            _context.Update(car);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Cars.Any(e => e.Id == id))
                return NotFound();
            else
                throw;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var car = await _context.Cars
            .Include(c => c.Owner)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (car == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && car.OwnerId != user.Id)
            return Forbid();

        return View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var car = await _context.Cars.FindAsync(id);
        if (car == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        if (!isAdmin && car.OwnerId != user.Id)
            return Forbid();

        _context.Cars.Remove(car);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
