using Hotel.Models;
using Hotel.Services;

namespace Hotel.Controllers
{
    public static class UserController
    {
        public static void MapUserController(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/user");

            group.MapGet("/role/{roleId}", async (int roleId, UserService service) =>
            {
                try
                {
                    return Results.Ok(await service.GetByRole(roleId));
        }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
            

            group.MapPost("/", async (User user, UserService service) =>
            {
                try
                {
                    return Results.Ok(await service.Create(user) ? Results.Created($"/api/user/{user.Id}", user) : Results.BadRequest());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
           

            group.MapGet("/", async (UserService service) =>
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
            

            group.MapDelete("/{id}", async (int id, UserService service) =>
            {
                try
                {
                    return Results.Ok(await service.Delete(id) ? Results.NoContent() : Results.NotFound());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });

            group.MapPut("/{id}", async (int id, User updatedUser, UserService service) =>
            {
                var result = await service.Update(id, updatedUser);
                try
                {
                    return result ? Results.Ok(updatedUser) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
            });
        }
    }
}
