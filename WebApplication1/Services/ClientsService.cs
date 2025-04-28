using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public class ClientsService : IClientsService
{
    private readonly string _connectionString;

    public ClientsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<ClientTripsDTO> GetTripsByUserId(int id)
    {
        // SQL: Gets all trips for a specific client
        string query =
            "SELECT trip.IdTrip, trip.Name, Description, DateFrom, DateTo, MaxPeople, RegisteredAt, PaymentDate FROM trip join Client_Trip on trip.IdTrip = Client_Trip.IdTrip where Client_Trip.IdClient = @ID";
        var trips = new List<ReservationOfTripDTO>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ID", id);
            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int? registeredAt = null;
                    int? paymentDate = null;
                    
                    int registeredAtOrdinal = reader.GetOrdinal("RegisteredAt");
                    if (!reader.IsDBNull(registeredAtOrdinal))
                    {
                        registeredAt = reader.GetInt32(registeredAtOrdinal);
                    }

                    int paymentDateOrdinal = reader.GetOrdinal("PaymentDate");
                    if (!reader.IsDBNull(paymentDateOrdinal))
                    {
                        paymentDate = reader.GetInt32(paymentDateOrdinal);
                    }

                    ReservationOfTripDTO newTrip = new ReservationOfTripDTO
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("IdTrip")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                        EndDate = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                        MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                        RegisteredAt = registeredAt,
                        PaymentDate = paymentDate,
                    };
                    trips.Add(newTrip);
                }
            }
        }

        return new ClientTripsDTO { ClientId = id, Trips = trips };
    }

    public async Task<Boolean> DoesClientExist(int id)
    {
        // SQL: Checks if client with given ID exists
        string query = "Select 1 from client where client.IdClient = @ID";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@ID", id);
            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }

    public async Task<int?> CreateNewClient(ClientDTO client)
    {
        // SQL: Inserts a new client and returns the ID of newly created client
        string query =
            "Insert into Client (FirstName, LastName, Email, Pesel, Telephone) " +
            "values (@FIRSTNAME, @LASTNAME, @EMAIL, @PESEL, @TELEPHONE); " +
            "SELECT CAST(SCOPE_IDENTITY() AS INT);";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@FIRSTNAME", client.FirstName);
            command.Parameters.AddWithValue("@LASTNAME", client.LastName);
            command.Parameters.AddWithValue("@EMAIL", client.Email);
            command.Parameters.AddWithValue("@PESEL", client.Pesel);
            command.Parameters.AddWithValue("@TELEPHONE", client.Telephone);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            if (result != null && int.TryParse(result.ToString(), out int newClientId))
            {
                return newClientId;
            }

            return null;
        }
    }
}