using PersonnelAccessManagement.Domain.Enums;

namespace PersonnelAccessManagement.Application.Features.Rules.Dtos;

public sealed record OverlappingRule(Campus? Campus, Title? Title, List<decimal> RoleIds);