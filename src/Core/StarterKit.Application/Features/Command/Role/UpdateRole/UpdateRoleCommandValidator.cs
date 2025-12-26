using FluentValidation;
using StarterKit.Application.Features.Commands.Role.UpdateRole;

namespace StarterKit.Application.Features.Command.Role.UpdateRole
{
    public class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommandRequest>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Rol adı boş ola bilməz")
                .MaximumLength(50).WithMessage("Rolun uzunluğu maksimum 50 ola bilər");
        }
    }
}
