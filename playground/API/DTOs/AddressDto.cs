using System.ComponentModel.DataAnnotations;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace API.DTOs;
public class AddressDto
{
    [Required(ErrorMessage = "First Name is required")]
    public string Fname { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string Lname { get; set; }

    [Required(ErrorMessage = "Street is required")]
    public string Street { get; set; }

    [Required(ErrorMessage = "City is required")]
    public string City { get; set; }

    [Required(ErrorMessage = "State is required")]
    public string State { get; set; }

    [Required(ErrorMessage = "Zip Code is required")]
    [RegularExpression(@"^\d{5}(?:-\d{4})?$", ErrorMessage = "Invalid ZIP code")]
    public string ZipCode { get; set; }
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.