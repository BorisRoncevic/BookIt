using Auth.Service.Models;

namespace Auth.Service.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}