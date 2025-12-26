using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.VerifyResetCode
{
    public class VerifyResetCodeCommandValidator : AbstractValidator<VerifyResetCodeCommandRequest>
    {
        public VerifyResetCodeCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");

            RuleFor(x => x.Otp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("VerificationCodeEmpty")
                .InclusiveBetween(100000, 999999).WithMessage("VerificationCodeInvalid");
        }
    }
}
