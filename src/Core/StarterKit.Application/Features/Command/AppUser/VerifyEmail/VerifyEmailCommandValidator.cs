using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.VerifyEmail
{
    public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommandRequest>
    {
        public VerifyEmailCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");

            RuleFor(x => x.VerificationCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("VerificationCodeEmpty")
                .InclusiveBetween(100000, 999999).WithMessage("VerificationCodeInvalid");
        }
    }
}