using System.ComponentModel.DataAnnotations;

namespace AzilEdu.Shared.DTOs;

public class SaveDonationDto : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Donator je obavezan.")]
    public int DonorId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Tip donacije je obavezan.")]
    public int DonationTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Status donacije je obavezan.")]
    public int DonationStatusId { get; set; }

    public DateTime DonationDate { get; set; } = DateTime.Today;

    [Range(0.01, 9999999, ErrorMessage = "Iznos mora biti veći od nule.")]
    public decimal? Amount { get; set; }

    [StringLength(200, ErrorMessage = "Naziv stvari može imati najviše 200 znakova.")]
    public string ItemName { get; set; } = string.Empty;

    [Range(0.01, 9999999, ErrorMessage = "Količina mora biti veća od nule.")]
    public decimal? Quantity { get; set; }

    [Range(0, 9999999, ErrorMessage = "Procijenjena vrijednost ne može biti negativna.")]
    public decimal? EstimatedValue { get; set; }

    [StringLength(1000, ErrorMessage = "Napomena može imati najviše 1000 znakova.")]
    public string Notes { get; set; } = string.Empty;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // 1 = Novčana u tablici DonationTypes
        var isMonetary = DonationTypeId == 1;

        if (isMonetary && Amount is null)
        {
            yield return new ValidationResult(
                "Novčana donacija mora imati iznos.",
                new[] { nameof(Amount) });
        }

        if (!isMonetary && DonationTypeId > 0 && string.IsNullOrWhiteSpace(ItemName))
        {
            yield return new ValidationResult(
                "Materijalna donacija mora imati naziv stvari ili usluge.",
                new[] { nameof(ItemName) });
        }

        if (!isMonetary && DonationTypeId > 0 && (Quantity is null || Quantity <= 0))
        {
            yield return new ValidationResult(
                "Materijalna donacija mora imati količinu veću od nule.",
                new[] { nameof(Quantity) });
        }

        if (DonationDate > DateTime.Today)
        {
            yield return new ValidationResult(
                "Datum donacije ne može biti u budućnosti.",
                new[] { nameof(DonationDate) });
        }
    }
}
