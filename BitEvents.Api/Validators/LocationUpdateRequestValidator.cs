using FluentValidation;
using BitEvents.Api.Contracts.Requests;

namespace BitEvents.Api.Validators;

public class LocationUpdateRequestValidator : AbstractValidator<LocationUpdateRequest>
{
    public LocationUpdateRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}