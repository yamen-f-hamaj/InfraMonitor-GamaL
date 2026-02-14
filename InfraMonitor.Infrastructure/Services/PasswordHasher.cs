using BCrypt.Net;
using InfraMonitor.Application.Common.Interfaces;



namespace InfraMonitor.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    => BCrypt.Net.BCrypt.HashPassword(password);


    public bool VerifyPassword(string password, string hash)
    => BCrypt.Net.BCrypt.Verify(password, hash);

}
