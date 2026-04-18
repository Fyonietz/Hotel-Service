using Hotel.Services;
using Hotel.Models;
namespace Hotel.Controllers
{
    public static class TestController
    {
        public static void MapTest(this WebApplication app)
        {
            var group = app.MapGroup("/api/v2/test");

            group.MapGet("/",async(TestServices services) =>{
                try
                {
                    var res = await services.GetData();
                    return Results.Ok(res);
                }catch(Exception e)
                {
                    return Results.InternalServerError(e.Message);
                }


            });
        }
    }
}
