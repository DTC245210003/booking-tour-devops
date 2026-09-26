using BookingTour.Data;
using BookingTour.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingTour.Controllers;

public class ToursController(AppDbContext db, ILogger<ToursController> logger) : Controller
{
    public async Task<IActionResult> Index() =>
        View(await db.Tours.OrderBy(t => t.Id).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var tour = await db.Tours.Include(t => t.Schedules)
                                 .FirstOrDefaultAsync(t => t.Id == id);
        return tour == null ? NotFound() : View(tour);
    }

    [HttpGet]
    public async Task<IActionResult> Book(int scheduleId)
    {
        var s = await LoadSchedule(scheduleId);
        if (s == null) return NotFound();
        ViewBag.Schedule = s;
        return View(new BookingForm { ScheduleId = scheduleId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Book(BookingForm form)
    {
        var s = await LoadSchedule(form.ScheduleId);
        if (s == null) return NotFound();
        if (form.NumPeople > s.Seats)
            ModelState.AddModelError(nameof(form.NumPeople), "Không đủ chỗ trống");
        if (!ModelState.IsValid) { ViewBag.Schedule = s; return View(form); }

        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Phone == form.Phone)
            ?? db.Customers.Add(new Customer
               { FullName = form.FullName, Phone = form.Phone, Email = form.Email }).Entity;

        s.Seats -= form.NumPeople;
        db.Bookings.Add(new Booking
        {
            Customer = customer, ScheduleId = s.Id,
            NumPeople = form.NumPeople, CreatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        logger.LogInformation("New booking: schedule {ScheduleId}, {NumPeople} people",
                              s.Id, form.NumPeople);
        TempData["Msg"] = "Đặt tour thành công!";
        return RedirectToAction(nameof(Details), new { id = s.TourId });
    }

    public async Task<IActionResult> Bookings() =>
        View(await db.Bookings.Include(b => b.Customer)
                              .Include(b => b.Schedule).ThenInclude(s => s!.Tour)
                              .OrderByDescending(b => b.CreatedAt).ToListAsync());

    private Task<Schedule?> LoadSchedule(int id) =>
        db.Schedules.Include(s => s.Tour).FirstOrDefaultAsync(s => s.Id == id);
}
