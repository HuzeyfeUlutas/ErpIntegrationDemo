using PersonnelAccessManagement.Application.Features.Rules.Dtos;
using Rule = PersonnelAccessManagement.Domain.Entities.Rule;

namespace PersonnelAccessManagement.Application.Features.Rules.Profile;

public sealed class RuleProfile : AutoMapper.Profile
{
    public RuleProfile()
    {
        CreateMap<Rule, RuleDto>();
    }
}