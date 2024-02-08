using FluentValidation;
using BitEvents.Api.Contracts.Requests;

namespace BitEvents.Api.Validators;

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}