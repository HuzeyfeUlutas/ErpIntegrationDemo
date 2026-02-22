using PersonnelAccessManagement.Domain.Entities;

namespace PersonnelAccessManagement.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(Personnel personnel);
}