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
                Results.Ok(await service.GetByRole(roleId)));

            group.MapPost("/", async (User user, UserService service) =>
                await service.Create(user) ? Results.Created($"/api/user/{user.Id}", user) : Results.BadRequest());

            group.MapGet("/", async (UserService service) =>
                Results.Ok(await service.GetAll()));

            group.MapDelete("/{id}", async (int id, UserService service) =>
                await service.Delete(id) ? Results.NoContent() : Results.NotFound());

            group.MapPut("/{id}", async (int id, User updatedUser, UserService service) =>
            {
                var result = await service.Update(id, updatedUser);

                return result ? Results.Ok(updatedUser) : Results.NotFound();
            });
        }
    }
}
