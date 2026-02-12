using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Profile;

public sealed class RuleProfile : AutoMapper.Profile
{
    public RuleProfile()
    {
        CreateMap<Rule, RuleDto>()
            .ForMember(d => d.Campus, o => o.MapFrom(s => s.Campus != null ? s.Campus.ToString() : null))
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title != null ? s.Title.ToString() : null))
            .ForMember(d => d.Roles, o => o.MapFrom(s => s.Roles));
    }
}