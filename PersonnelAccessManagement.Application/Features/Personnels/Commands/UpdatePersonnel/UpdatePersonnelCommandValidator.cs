using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Personnels.Commands.UpdatePersonnel;

public sealed class UpdatePersonnelCommandValidator : AbstractValidator<UpdatePersonnelCommand>
{
    public UpdatePersonnelCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.RoleIds)
            .NotNull();

        RuleForEach(x => x.RoleIds)
            .GreaterThan(0);

        RuleFor(x => x.RoleIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate role ids are not allowed.");
    }
}