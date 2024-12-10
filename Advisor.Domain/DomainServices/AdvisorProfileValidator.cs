using Advisor.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Advisor.Domain.DomainServices;
public class AdvisorProfileValidator : IModelValidator<AdvisorProfile>
{
    public void Validate(AdvisorProfile model)
    {
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model), "AdvisorProfile cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(model.FullName) || model.FullName.Length > 255)
        {
            throw new ValidationException("FullName is required and must be less than 255 characters.");
        }

        if (string.IsNullOrWhiteSpace(model.SIN) || model.SIN.Length != 9 || !model.SIN.All(char.IsDigit))
        {
            throw new ValidationException("SIN is required, must be exactly 9 digits.");
        }

        if (!string.IsNullOrWhiteSpace(model.Address) && model.Address.Length > 255)
        {
            throw new ValidationException("Address must be less than 255 characters.");
        }

        if (!string.IsNullOrWhiteSpace(model.PhoneNumber) && model.PhoneNumber.Length < 10)
        {
            throw new ValidationException("PhoneNumber must be 10 characters or more.");
        }
    }
}
