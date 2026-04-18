using Hotel.Controllers;
using Hotel.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<Database>();
builder.Services.AddScoped<TestServices>();
var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapTest();
app.Run();

