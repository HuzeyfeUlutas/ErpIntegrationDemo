using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.CreateRule;

public sealed class CreateRuleCommandValidator : AbstractValidator<CreateRuleCommand>
{
    public CreateRuleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RoleIds).NotNull().NotEmpty();
        RuleForEach(x => x.RoleIds).NotEmpty();
    }
}