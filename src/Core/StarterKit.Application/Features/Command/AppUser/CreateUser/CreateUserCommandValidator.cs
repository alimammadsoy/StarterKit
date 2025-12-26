using FluentValidation;
using StarterKit.Application.Features.Command.AppUser.RegisterUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarterKit.Application.Features.Command.AppUser.CreateUser
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommandRequest>
    {
        public CreateUserCommandValidator()
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

            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PasswordRequired")
                .MinimumLength(8).WithMessage("PasswordMinLength")
                .Matches(@"[A-Z]+").WithMessage("PasswordUppercaseRequired")    // ən az 1 böyük hərf
                .Matches(@"[a-z]+").WithMessage("PasswordLowercaseRequired")    // ən az 1 kicik hərf
                .Matches(@"[!@#$%^&*()_+\[\]{}|;:,.<>/?]").WithMessage("PasswordSpecialCharRequired"); // ən az 1 xüsusi simvol

            RuleFor(x => x.PasswordConfirmation)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PasswordConfirmationRequired")
                .Equal(x => x.Password).WithMessage("PasswordConfirmationNotMatch");

            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("RoleIdRequired");
        }
    }
}
