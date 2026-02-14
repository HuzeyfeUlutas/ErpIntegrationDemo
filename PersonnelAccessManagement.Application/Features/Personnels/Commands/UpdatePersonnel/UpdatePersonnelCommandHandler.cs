using MediatR;
using Microsoft.EntityFrameworkCore;
using PersonnelAccessManagement.Application.Common.Exceptions;
using PersonnelAccessManagement.Application.Common.Interfaces;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Personnels.Commands.UpdatePersonnel;


public sealed class UpdatePersonnelCommandHandler : IRequestHandler<UpdatePersonnelCommand>
{
    private readonly IRepository<Personnel> _personnels;
    private readonly IRepository<Role> _roles;
    private readonly IUnitOfWork _uow;

    public UpdatePersonnelCommandHandler(
        IRepository<Personnel> personnels,
        IRepository<Role> roles,
        IUnitOfWork uow)
    {
        _personnels = personnels;
        _roles = roles;
        _uow = uow;
    }

    public async Task Handle(UpdatePersonnelCommand request, CancellationToken ct)
    {
        var personnel = await _personnels.Query()
            .Include(r => r.Roles)
            .FirstOrDefaultAsync(r => r.EmployeeNo == request.EmployeeNo, ct);

        if (personnel is null)
            throw new NotFoundException($"Personnel not found: {request.EmployeeNo}");

        var roles = await _roles.Query()
            .Where(r => request.RoleIds.Contains(r.Id))
            .ToListAsync(ct);

        if (roles.Count != request.RoleIds.Count)
            throw new NotFoundException("One or more roles were not found.");
        
        personnel.SetRoles(roles);
        
 
        await _uow.SaveChangesAsync(ct);
    }
}