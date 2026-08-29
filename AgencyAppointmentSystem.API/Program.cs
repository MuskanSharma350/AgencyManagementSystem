using AgencyAppointmentSystem.Business.Interfaces;
using AgencyAppointmentSystem.Business.Services;
using AgencyAppointmentSystem.Data.Context;
using AgencyAppointmentSystem.Data.Repositories;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);
// Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
// Dependency Injection
builder.Services.AddScoped<IAppointmentRepository,
    AppointmentRepository>();

builder.Services.AddScoped<IHolidayRepository,
    HolidayRepository>();

builder.Services.AddScoped<IAppointmentService,
    AppointmentService>();
builder.Services.AddScoped<IHolidayService,
    HolidayService>();
builder.Services.AddScoped<IAgencySettingsService,
    AgencySettingsService>();
builder.Services.AddScoped<IAgencySettingsRepository,
    AgencySettingsRepository>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.Run();  