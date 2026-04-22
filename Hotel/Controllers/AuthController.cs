using Hotel.Models;
using Hotel.Services;

namespace Hotel.Controllers
{
    public static class AuthController
    {
        public static void MapAuthController(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("api/v1/auth");

            group.MapPost("/login", async (User loginData, AuthService service) =>
            {
                try {

                    var user = await service.Login(
                       loginData.Email ?? "",
                       loginData.Password ?? ""
                   );

                    if (user == null)
                        return Results.Json(new { message = "Gagal Login" }, statusCode: 401);

                    return Results.Ok(new
                    {
                        
                            user.Id,
                            user.Name,
                            user.Email,
                            user.RoleId
                        
                    });


                } catch (Exception ex)
                {
                    return Results.Json(new { message = "Terjadi kesalahan: " + ex.Message }, statusCode: 500);
                }
               
            });
        }
    }
}