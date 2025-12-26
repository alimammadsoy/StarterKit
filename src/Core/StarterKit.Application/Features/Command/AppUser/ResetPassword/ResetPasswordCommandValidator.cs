using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommandRequest>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");

            RuleFor(x => x.Otp)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("VerificationCodeEmpty")
                .InclusiveBetween(100000, 999999).WithMessage("VerificationCodeInvalid");

            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PasswordRequired")
                .MinimumLength(8).WithMessage("PasswordMinLength")
                .Matches(@"[A-Z]+").WithMessage("PasswordUppercaseRequired")    // ən az 1 böyük hərf
                .Matches(@"[a-z]+").WithMessage("PasswordLowercaseRequired")    // ən az 1 kicik hərf
                .Matches(@"[!@#$%^&*()_+\[\]{}|;:,.<>/?]").WithMessage("PasswordSpecialCharRequired"); // ən az 1 xüsusi simvol

            RuleFor(x => x.NewPasswordConfirmation)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PasswordConfirmationRequired")
                .Equal(x => x.NewPassword).WithMessage("PasswordConfirmationNotMatch");
        }
    }
}
