using Hotel.Controllers;
using Hotel.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
var app = builder.Build();



// Configure the HTTP request pipeline.

app.MapAuthController();
app.MapUserController();
app.Run();

