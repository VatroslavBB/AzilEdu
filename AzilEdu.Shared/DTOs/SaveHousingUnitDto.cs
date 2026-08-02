using System.ComponentModel.DataAnnotations;

namespace AzilEdu.Shared.DTOs;

public class SaveHousingUnitDto : IValidatableObject
{
    [Required(ErrorMessage = "Naziv je obavezan.")]
    [StringLength(100, ErrorMessage = "Naziv može imati najviše 100 znakova.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tip jedinice je obavezan.")]
    [StringLength(100, ErrorMessage = "Tip jedinice može imati najviše 100 znakova.")]
    public string UnitType { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Kapacitet mora biti između 1 i 1000.")]
    public int Capacity { get; set; }

    [Range(0, 1000, ErrorMessage = "Zauzeće ne može biti negativno.")]
    public int Occupied { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastCleanedAt { get; set; }

    [StringLength(300, ErrorMessage = "Putanja slike može imati najviše 300 znakova.")]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Napomena može imati najviše 1000 znakova.")]
    public string Note { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Occupied > Capacity)
        {
            yield return new ValidationResult(
                "Zauzeće ne može biti veće od kapaciteta.",
                new[] { nameof(Occupied) });
        }

        if (LastCleanedAt > DateTime.Today)
        {
            yield return new ValidationResult(
                "Datum čišćenja ne može biti u budućnosti.",
                new[] { nameof(LastCleanedAt) });
        }
    }
}
