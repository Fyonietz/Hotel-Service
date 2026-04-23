using Hotel.Models;
using Hotel.Services;

namespace Hotel.Controllers
{
    public static class RoomController
    {
        public static void MapRoomController(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/room");

            group.MapGet("/{roomId}", async (int roomId, RoomService service) =>
            {
                try
                {
                   return Results.Ok(await service.GetById(roomId));
                }
                catch(Exception ex)
                {
                    return Results.InternalServerError(ex.Message); 
                }
            });

            group.MapGet("/", async (RoomService service) =>

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
        

            group.MapPost("/", async (Room room, RoomService service) =>
            {
                try
                {
                    return Results.Ok(await service.Create(room) ? Results.Created($"/api/v1/room/{room.Id}", room) : Results.BadRequest());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
           

            group.MapDelete("/{roomId}", async (int roomId, RoomService service) =>
            {
                try
                {
                    return Results.Ok(await service.Delete(roomId));
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapPut("/{id}", async (int id, Room updatedRoom, RoomService service) =>
            {
                var result = await service.Update(id, updatedRoom);

                try
                {
                    return result ? Results.Ok(updatedRoom) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

        }
    }
}