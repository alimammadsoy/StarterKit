using FluentValidation;

namespace StarterKit.Application.Features.Command.AppUser.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommandRequest>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("NameRequired")
                .MaximumLength(50).WithMessage("NameMaxLength");

            RuleFor(x => x.Surname)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("SurnameRequired")
                .MaximumLength(50).WithMessage("SurnameMaxLength");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("EmailRequired")
                .EmailAddress().WithMessage("EmailInvalid");

            RuleFor(x => x.Phone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PhoneRequired")
                .Matches(@"^\+?[0-9]{7,14}$").WithMessage("PhoneInvalid");
        }
    }
}
