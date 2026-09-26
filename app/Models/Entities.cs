using System.ComponentModel.DataAnnotations;

namespace BookingTour.Models;

public class Tour
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Destination { get; set; } = "";
    public int Days { get; set; }
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public List<Schedule> Schedules { get; set; } = new();
}

public class Schedule
{
    public int Id { get; set; }
    public int TourId { get; set; }
    public Tour? Tour { get; set; }
    public DateOnly StartDate { get; set; }
    public int Seats { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string? Email { get; set; }
}

public class Booking
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int ScheduleId { get; set; }
    public Schedule? Schedule { get; set; }
    public int NumPeople { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BookingForm
{
    public int ScheduleId { get; set; }
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100)]
    public string FullName { get; set; } = "";
    [Required(ErrorMessage = "Nhập số điện thoại"), Phone, StringLength(20)]
    public string Phone { get; set; } = "";
    [EmailAddress] public string? Email { get; set; }
    [Range(1, 50, ErrorMessage = "Số người từ 1 đến 50")]
    public int NumPeople { get; set; } = 1;
}
