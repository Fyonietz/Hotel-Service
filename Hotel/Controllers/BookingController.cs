using Hotel.Models;
using Hotel.Services;

namespace Hotel.Controllers
{
    public static class BookingController
    {
        public static void MapBookingController(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/booking");
            group.MapGet("/{bookingId}", async (int bookingId, BookingService service) =>
            {
                try
                {
                    return Results.Ok(await service.GetById(bookingId));
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
            group.MapGet("/", async (BookingService service) =>
            {
                try
                {
                    return Results.Ok(await service.GetAll());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
            group.MapPost("/", async (Booking booking, BookingService service) =>
            {
                try
                {
                    return Results.Ok(await service.Create(booking) ? Results.Created($"/api/v1/booking/{booking.Id}", booking) : Results.BadRequest());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
            group.MapDelete("/{bookingId}", async (int bookingId, BookingService service) =>
            {
                try
                {
                    return Results.Ok(await service.Delete(bookingId));
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
            group.MapPut("/{bookingId}/{statusId}", async (int bookingId, int statusId, BookingService service) =>
            {
                try
                {
                    return Results.Ok(await service.Update(bookingId, statusId) ? Results.NoContent() : Results.NotFound());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
        }
    }
}
