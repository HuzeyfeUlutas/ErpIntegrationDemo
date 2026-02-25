using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Rules.Commands.DeleteRule;

public sealed class DeleteRuleCommandValidator : AbstractValidator<DeleteRuleCommand>
{
    public DeleteRuleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Kural ID zorunludur.");
    }
}