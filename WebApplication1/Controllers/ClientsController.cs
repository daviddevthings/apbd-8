using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController(IClientsService clientsService, ITripsService tripsService) : ControllerBase
    {
        // GET /api/clients/{id}/trips - Retrieves all trips associated with a specific client
        [HttpGet("{id:int}/trips")]
        public async Task<IActionResult> GetTrip(int id)
        {
            if (!await clientsService.DoesClientExist(id))
            {
                return NotFound(new { message = $"Client with ID {id} not found" });
            }

            var trips = await clientsService.GetTripsByUserId(id);
            if (trips.Trips.Count == 0)
            {
                return NotFound(new { message = $"Client with ID {id} does not have any trips reserved" });
            }

            return Ok(trips);
        }

        // POST /api/clients - Creates a new client record in the database
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientDTO? clientDto)
        {
            if (clientDto == null)
            {
                return BadRequest(new { message = "Client data is null" });
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { message = "Invalid client data", errors });
            }

            var clientID = await clientsService.CreateNewClient(clientDto);
            return clientID == null ? StatusCode(500, new { message = "Failed to create client" }) : StatusCode(201, new { message = $"Client {clientID} created successfully" });
        }

        // PUT /api/clients/{id}/trips/{tripId} - Registers a client for a specific trip
        [HttpPut("{id:int}/trips/{tripId:int}")]
        public async Task<IActionResult> RegisterClientForTrip(int id, int tripId)
        {
            if (!await clientsService.DoesClientExist(id))
            {
                return NotFound(new { message = $"Client with ID {id} not found" });
            }

            if (!await tripsService.DoesTripExist(tripId))
            {
                return NotFound(new { message = $"Trip with ID {tripId} not found" });
            }
            
            if (!await tripsService.HasAvailableSpots(tripId))
            {
                return BadRequest(new { message = $"Trip with ID {tripId} has reached maximum number of participants" });
            }

            var status = await tripsService.RegisterForTrip(id, tripId);
            return status ? Ok(new { message = $"Client with ID {id} registered successfully for trip {tripId}" }) : StatusCode(500, new { message = "Failed to register client for trip" });
        }

        // DELETE /api/clients/{id}/trips/{tripId} - Removes a client's registration from a trip
        [HttpDelete("{id:int}/trips/{tripId:int}")]
        public async Task<IActionResult> UnregisterClientFromTrip(int id, int tripId)
        {
            if (!await clientsService.DoesClientExist(id))
            {
                return NotFound(new { message = $"Client with ID {id} not found" });
            }

            if (!await tripsService.DoesTripExist(tripId))
            {
                return NotFound(new { message = $"Trip with ID {tripId} not found" });
            }

            if (!await tripsService.DoesRegistrationExist(id, tripId))
            {
                return NotFound(new { message = $"Registration of client {id} for trip {tripId} not found" });
            }

            var status = await tripsService.UnregisterFromTrip(id, tripId);
            return status ? Ok(new { message = $"Unregistered client {id} from trip {tripId}" }) : StatusCode(500, new { message = "Failed to unregister client from trip" });
        }
    }
}