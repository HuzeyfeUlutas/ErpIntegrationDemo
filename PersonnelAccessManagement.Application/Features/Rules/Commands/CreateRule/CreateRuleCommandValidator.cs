using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;

public sealed class CreateRuleCommandValidator : AbstractValidator<CreateRuleCommand>
{
    public CreateRuleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Kural adı zorunludur.")
            .MaximumLength(200).WithMessage("Kural adı en fazla 200 karakter olabilir.");
        RuleFor(x => x.RoleIds).NotNull().WithMessage("Rol listesi zorunludur.")
            .NotEmpty().WithMessage("En az bir rol seçilmelidir.");
        RuleForEach(x => x.RoleIds).NotEmpty().WithMessage("Rol ID boş olamaz.");
    }
}