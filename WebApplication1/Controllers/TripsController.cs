using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using Microsoft.Data.SqlClient;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripsController(ITripsService tripsService) : ControllerBase
    {
        // GET /api/trips - Retrieves all available trips with their details
        [HttpGet]
        public async Task<IActionResult> GetTrips()
        {
            var trips = await tripsService.GetTrips();
            return Ok(trips);
        }
    }
}