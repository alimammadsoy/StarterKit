using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.ResendVerificationCode
{
    public class ResendVerificationCodeCommandValidator : AbstractValidator<ResendVerificationCodeCommandRequest>
    {
        public ResendVerificationCodeCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");
        }
    }
}
