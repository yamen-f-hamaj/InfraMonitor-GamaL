using InfraMonitor.Domain.Entities;

namespace InfraMonitor.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
