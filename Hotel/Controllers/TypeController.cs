using Hotel.Models;
using Hotel.Services;

namespace Hotel.Controllers
{
    public static class TypeController
    {
        public static void MapTypeController(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/types");

            // GET ALL
            group.MapGet("/", async (TypeService service) =>
            {
                try
                {
                    var types = await service.GetAll();
                    return Results.Ok(types);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // GET BY ID
            group.MapGet("/{typeId}", async (int typeId, TypeService service) =>
            {
                try
                {
                    var type = await service.GetById(typeId);
                    return type is null ? Results.NotFound() : Results.Ok(type);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // POST
            group.MapPost("/", async (Types type, TypeService service) =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(type.Name))
                        return Results.BadRequest(new { message = "Name is required" });

                    var success = await service.Create(type);

                    return success
                        ? Results.Created($"/api/v1/types/{type.Id}", type)
                        : Results.BadRequest();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });

            // DELETE
            group.MapDelete("/{typeId}", async (int typeId, TypeService service) =>
            {
                try
                {
                    var success = await service.Delete(typeId);

                    return success
                        ? Results.Ok(new { message = "Deleted" })
                        : Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            });
        }
    }
}