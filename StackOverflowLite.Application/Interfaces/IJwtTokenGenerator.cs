using StackOverflowLite.Application.Models;

namespace StackOverflowLite.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(ApplicationUser user);
}
