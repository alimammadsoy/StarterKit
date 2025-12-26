using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.ResendResetCode
{
    public class ResendResetCodeCommandValidator : AbstractValidator<ResendResetCodeCommandRequest>
    {
        public ResendResetCodeCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");
        }
    }
}