using FluentValidation;
using StarterKit.Application.Features.Commands.Role.CreateRole;

namespace StarterKit.Application.Features.Command.Role.CreateRole
{
    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommandRequest>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol adı boş ola bilməz")
                .MaximumLength(50).WithMessage("Rolun uzunluğu maksimum 50 ola bilər");
        }
    }
}
