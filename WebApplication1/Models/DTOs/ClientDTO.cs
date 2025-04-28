namespace WebApplication1.Models.DTOs;

using System.ComponentModel.DataAnnotations;

public class ClientDTO
{
    [Required(ErrorMessage = "First name is required.")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Telephone is required.")]
    [RegularExpression(@"^\+?[0-9]{9,15}$", ErrorMessage = "Invalid telephone number format.")]
    public string Telephone { get; set; }

    [Required(ErrorMessage = "Pesel is required.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Invalid PESEL format.")]
    public string Pesel { get; set; }
}
