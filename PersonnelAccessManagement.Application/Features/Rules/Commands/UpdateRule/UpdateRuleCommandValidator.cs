using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.UpdateRule;

public sealed class UpdateRuleCommandValidator : AbstractValidator<UpdateRuleCommand>
{
    public UpdateRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RoleIds).NotNull().NotEmpty();
        RuleForEach(x => x.RoleIds).NotEmpty();
    }
}