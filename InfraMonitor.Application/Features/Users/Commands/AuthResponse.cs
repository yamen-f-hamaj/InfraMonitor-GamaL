namespace InfraMonitor.Application.Features.Users.Commands;

public record AuthResponse(int UserId, string UserName, string Email, string Role, string Token);
