using System.Text.Json.Serialization;

namespace WebApplication1.Models.DTOs;

public class ClientTripsDTO
{
    public int ClientId { get; set; }
    public List<ReservationOfTripDTO> Trips { get; set; }
}

public class ReservationOfTripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
    public int? RegisteredAt { get; set; }
    public int? PaymentDate { get; set; }
}