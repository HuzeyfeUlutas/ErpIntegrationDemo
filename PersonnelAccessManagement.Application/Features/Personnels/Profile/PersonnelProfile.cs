using PersonnelAccessManagement.Application.Features.Personnels.Dtos;
using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Features.Personnels.Profile;

public sealed class PersonnelProfile : AutoMapper.Profile
{
    public PersonnelProfile()
    {
        CreateMap<Personnel, PersonnelDto>();
    }
}