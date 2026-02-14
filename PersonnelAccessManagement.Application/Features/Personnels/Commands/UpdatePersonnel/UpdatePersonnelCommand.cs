using MediatR;

namespace PersonnelAccessManagement.Application.Features.Personnels.Commands.UpdatePersonnel;

public sealed record UpdatePersonnelCommand(
    decimal EmployeeNo,
    List<decimal> RoleIds
) : IRequest;