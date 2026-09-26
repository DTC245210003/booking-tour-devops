using BookingTour.Data;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
     .UseSnakeCaseNamingConvention());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseExceptionHandler("/Home/Error");

app.UseRouting();
app.UseHttpMetrics();

app.MapStaticAssets();
app.MapControllerRoute("default", "{controller=Tours}/{action=Index}/{id?}")
   .WithStaticAssets();
app.MapMetrics();

app.Run();
