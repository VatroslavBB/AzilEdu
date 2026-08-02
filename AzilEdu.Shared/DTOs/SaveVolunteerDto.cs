using System.ComponentModel.DataAnnotations;

namespace AzilEdu.Shared.DTOs;

public class SaveVolunteerDto
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(100, ErrorMessage = "Ime može imati najviše 100 znakova.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prezime je obavezno.")]
    [StringLength(100, ErrorMessage = "Prezime može imati najviše 100 znakova.")]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Upiši ispravnu e-mail adresu.")]
    [StringLength(200, ErrorMessage = "E-mail može imati najviše 200 znakova.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Telefon može imati najviše 50 znakova.")]
    public string Phone { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Vještine mogu imati najviše 500 znakova.")]
    public string Skills { get; set; } = string.Empty;

    public DateTime? AvailableFrom { get; set; }

    [StringLength(1000, ErrorMessage = "Napomena može imati najviše 1000 znakova.")]
    public string Notes { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Status volontera je obavezan.")]
    public int VolunteerStatusId { get; set; }
}
