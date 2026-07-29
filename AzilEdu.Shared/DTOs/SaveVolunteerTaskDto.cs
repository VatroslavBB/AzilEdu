using System.ComponentModel.DataAnnotations;

namespace AzilEdu.Shared.DTOs;

public class SaveVolunteerTaskDto : IValidatableObject
{
    [Required(ErrorMessage = "Naslov zadatka je obavezan.")]
    [StringLength(200, ErrorMessage = "Naslov može imati najviše 200 znakova.")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Opis može imati najviše 1000 znakova.")]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }

    [StringLength(1000, ErrorMessage = "Napomena može imati najviše 1000 znakova.")]
    public string Notes { get; set; } = string.Empty;

    public int? VolunteerId { get; set; }
    public int? AnimalId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Status zadatka je obavezan.")]
    public int VolunteerTaskStatusId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Tip zadatka je obavezan.")]
    public int VolunteerTaskTypeId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 4 = Završeno u tablici VolunteerTaskStatuses
        var isCompleted = VolunteerTaskStatusId == 4;

        if (isCompleted && CompletedAt is null)
        {
            yield return new ValidationResult(
                "Zadatak sa statusom \"Završeno\" mora imati datum završetka.",
                new[] { nameof(CompletedAt) });
        }

        if (!isCompleted && CompletedAt is not null)
        {
            yield return new ValidationResult(
                "Datum završetka smije se unijeti samo kad je status \"Završeno\".",
                new[] { nameof(CompletedAt) });
        }
    }
}
