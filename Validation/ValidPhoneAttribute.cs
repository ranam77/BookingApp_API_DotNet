 
using System.ComponentModel.DataAnnotations;
using PhoneNumbers;
namespace BookingApi.Validation;

public class ValidPhoneAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null || string.IsNullOrWhiteSpace(value.ToString()))
            return ValidationResult.Success; // Required handles empty values

        try
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();

            var phoneNumber = phoneUtil.Parse(
                value.ToString(),
                null
            );

            if (phoneUtil.IsValidNumber(phoneNumber))
                return ValidationResult.Success;
        }
        catch (NumberParseException)
        {
            // Invalid phone format
        }

        return new ValidationResult(
            "Please enter a valid international phone number."
        );
    }
}