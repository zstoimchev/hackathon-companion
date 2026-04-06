using HackathonOS.Application.Interfaces;

namespace HackathonOS.Application.Services;

public class HashingService : IHashingService
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}