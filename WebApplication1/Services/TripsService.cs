using Microsoft.Data.SqlClient;
using WebApplication1.Models.DTOs;

namespace WebApplication1.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString;

    public TripsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new Dictionary<int, TripDTO>();

        // SQL: Gets all trips with their countries
        string command = @"
            SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name as CountryName 
            FROM trip t
            JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip
            JOIN Country c ON c.IdCountry = ct.IdCountry";

        using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
        using (SqlCommand cmd = new SqlCommand(command, sqlConnection))
        {
            await sqlConnection.OpenAsync();
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int tripId = reader.GetInt32(reader.GetOrdinal("IdTrip"));
                    string countryName = reader.GetString(reader.GetOrdinal("CountryName"));

                    if (trips.ContainsKey(tripId))
                    {
                        TripDTO existingTrip = trips[tripId];
                        existingTrip.Countries.Add(new CountryDTO { Name = countryName });
                    }
                    else
                    {
                        TripDTO newTrip = new TripDTO
                        {
                            Id = tripId,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.GetString(reader.GetOrdinal("Description")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("DateFrom")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("DateTo")),
                            MaxPeople = reader.GetInt32(reader.GetOrdinal("MaxPeople")),
                            Countries = new List<CountryDTO>()
                        };

                        newTrip.Countries.Add(new CountryDTO { Name = countryName });

                        trips.Add(tripId, newTrip);
                    }
                }
            }
        }

        return trips.Values.ToList();
    }

    public async Task<Boolean> DoesTripExist(int id)
    {
        // SQL: Checks if a trip with the given ID exists
        string query = "Select 1 from Trip where Trip.IdTrip = @ID";
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

    public async Task<Boolean> RegisterForTrip(int clientId, int tripId)
    {
        // SQL: Inserts a record into Client_Trip table to register client for trip
        string query =
            "insert into Client_Trip (IdClient, IdTrip, RegisteredAt, PaymentDate) values (@CLIENTID,@TRIPID,@REGISTEREDAT,null)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            var currentDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
            command.Parameters.AddWithValue("@CLIENTID", clientId);
            command.Parameters.AddWithValue("@TRIPID", tripId);
            command.Parameters.AddWithValue("@REGISTEREDAT", currentDate);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<Boolean> DoesRegistrationExist(int clientId, int tripId)
    {
        // SQL: Checks if client is already registered for trip
        string query = "Select 1 from Client_Trip where IdClient=@CLIENTID and IdTrip=@TRIPID";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@CLIENTID", clientId);
            command.Parameters.AddWithValue("@TRIPID", tripId);
            await connection.OpenAsync();
            using (var reader = await command.ExecuteReaderAsync())
            {
                return await reader.ReadAsync();
            }
        }
    }

    public async Task<Boolean> UnregisterFromTrip(int clientId, int tripId)
    {
        // SQL: Deletes client registration from trip
        string query = "delete from Client_Trip where IdClient=@CLIENTID and IdTrip=@TRIPID";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@CLIENTID", clientId);
            command.Parameters.AddWithValue("@TRIPID", tripId);
            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync() > 0;
        }
    }

    public async Task<Boolean> HasAvailableSpots(int tripId)
    {
        // SQL: Checks if trip has not reached maximum number of participants
        string query = @"
            SELECT 
                CASE WHEN COUNT(ct.IdClient) < t.MaxPeople THEN 1 ELSE 0 END AS HasSpots
            FROM Trip t
            LEFT JOIN Client_Trip ct ON t.IdTrip = ct.IdTrip
            WHERE t.IdTrip = @TRIPID
            GROUP BY t.MaxPeople";
            
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@TRIPID", tripId);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            
            if (result == null)
                return false;
                
            return Convert.ToBoolean(result);
        }
    }
}