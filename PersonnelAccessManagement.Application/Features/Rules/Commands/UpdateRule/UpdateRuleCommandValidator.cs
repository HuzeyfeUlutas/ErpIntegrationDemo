using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;

public sealed class UpdateRuleCommandValidator : AbstractValidator<UpdateRuleCommand>
{
    public UpdateRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Kural ID zorunludur.");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Kural adı zorunludur.")
            .MaximumLength(200).WithMessage("Kural adı en fazla 200 karakter olabilir.");
        RuleFor(x => x.RoleIds).NotNull().WithMessage("Rol listesi zorunludur.")
            .NotEmpty().WithMessage("En az bir rol seçilmelidir.");
        RuleForEach(x => x.RoleIds).NotEmpty().WithMessage("Rol ID boş olamaz.");
    }
}