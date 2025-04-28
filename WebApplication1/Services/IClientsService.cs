using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public interface IClientsService
{
    public Task<ClientTripsDTO> GetTripsByUserId(int id);
    public Task<Boolean> DoesClientExist(int id);
    public Task<int?> CreateNewClient(ClientDTO client);
}