using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Personnels.Commands.UpdatePersonnel;

public sealed class UpdatePersonnelCommandValidator : AbstractValidator<UpdatePersonnelCommand>
{
    public UpdatePersonnelCommandValidator()
    {
        RuleFor(x => x.EmployeeNo)
            .NotEmpty().WithMessage("Sicil numarası zorunludur.");

        RuleFor(x => x.RoleIds)
            .NotNull().WithMessage("Rol listesi zorunludur.");

        RuleForEach(x => x.RoleIds)
            .GreaterThan(0).WithMessage("Rol ID sıfırdan büyük olmalıdır.");

        RuleFor(x => x.RoleIds)
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Aynı rol birden fazla kez eklenemez.");
    }
}