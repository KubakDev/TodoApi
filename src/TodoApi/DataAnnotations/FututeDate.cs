using System.ComponentModel.DataAnnotations;

namespace TodoApi.DataAnnotations;

public class FutureDateAttribute : ValidationAttribute
{
  protected override ValidationResult IsValid(object value, ValidationContext validationContext)
  {
    DateTime _dateFuture = Convert.ToDateTime(value);
    if (_dateFuture >= DateTime.Now)
    {
      return ValidationResult.Success;
    }
    else
    {
      return new ValidationResult(ErrorMessage);
    }
  }
}