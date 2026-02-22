using FluentValidation;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmployeeNo)
            .NotEmpty().WithMessage("Sicil numarası zorunludur.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.");
    }
}