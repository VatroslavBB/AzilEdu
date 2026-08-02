using System.ComponentModel.DataAnnotations;

namespace AzilEdu.Shared.DTOs;

public class SaveAnimalDto
{
    [Required(ErrorMessage = "Ime je obavezno.")]
    [StringLength(100, ErrorMessage = "Ime može imati najviše 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vrsta je obavezna.")]
    [StringLength(100, ErrorMessage = "Vrsta može imati najviše 100 znakova.")]
    public string Species { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Pasmina može imati najviše 100 znakova.")]
    public string Breed { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Spol može imati najviše 50 znakova.")]
    public string Gender { get; set; } = string.Empty;

    [Range(0, 60, ErrorMessage = "Dob mora biti između 0 i 60 godina.")]
    public int? Age { get; set; }

    public DateTime? ArrivalDate { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Status životinje je obavezan.")]
    public int AnimalStatusId { get; set; }

    [StringLength(300, ErrorMessage = "Putanja slike može imati najviše 300 znakova.")]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Opis može imati najviše 1000 znakova.")]
    public string Description { get; set; } = string.Empty;
}
