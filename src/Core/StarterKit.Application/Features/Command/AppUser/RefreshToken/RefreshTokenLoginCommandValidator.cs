using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.RefreshToken
{
    public class RefreshTokenLoginCommandValidator : AbstractValidator<RefreshTokenLoginCommandRequest>
    {
        public RefreshTokenLoginCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshTokenRequired");
        }
    }
}
