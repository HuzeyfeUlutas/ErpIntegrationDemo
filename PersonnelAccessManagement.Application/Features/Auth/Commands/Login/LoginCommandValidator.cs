using FluentValidation;
using PersonnelAccessManagement.Application.Features.Auth.Commands.Login;

namespace PersonnelAccessManagement.Application.Features.Auth.Commands;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmployeeNo)
            .NotEmpty().WithMessage("Sicil numarası zorunludur.");
        
        RuleFor(x => x.EmployeeNo)
            .Must(x => decimal.TryParse(x, out _)).WithMessage("Geçerli bir sicil numarası girin.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.");
    }
}