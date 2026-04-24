using Hotel.Controllers;
using Hotel.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Waiter API v1");
    options.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.

app.MapAuthController();
app.MapUserController();
app.MapRoomController();
app.MapBookingController();
app.Run();

