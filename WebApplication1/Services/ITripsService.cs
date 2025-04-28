using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface ITripsService
{
    public Task<List<TripDTO>> GetTrips();
    public Task<Boolean> DoesTripExist(int id);
    public Task<Boolean> RegisterForTrip(int clientId, int tripId);
    public Task<Boolean> DoesRegistrationExist(int clientId, int tripId);
    public Task<Boolean> UnregisterFromTrip(int clientId, int tripId);
    public Task<Boolean> HasAvailableSpots(int tripId);
}