using BookingTour.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingTour.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Booking> Bookings => Set<Booking>();
}
