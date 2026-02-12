using PersonnelAccessManagement.Application.Features.Roles.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Roles.Profile;


public sealed class RoleProfile : AutoMapper.Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDto>();
    }
}