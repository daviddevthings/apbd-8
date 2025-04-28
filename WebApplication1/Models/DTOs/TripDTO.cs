namespace WebApplication1.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO>? Countries { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}
