using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.LogoutUser
{
    public class LogoutCommandValidator : AbstractValidator<LogoutCommandRequest>
    {
        public LogoutCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("RefreshTokenRequired");
        }
    }
}