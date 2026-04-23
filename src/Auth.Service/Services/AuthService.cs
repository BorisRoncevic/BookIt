using Auth.Service.Models;

namespace Auth.Service.Services;

public class AuthService
{
    private static readonly List<User> Users = new();
    private readonly ITokenService _tokenService;

    public AuthService(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public Task<string> RegisterAsync(RegisterRequest request)
    {
        var existingUser = Users.FirstOrDefault(
            user => user.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)
        );

        if (existingUser != null)
            throw new InvalidOperationException("User with this email already exists.");

        var user = new User
        {
            Id = Users.Count + 1,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = "Customer",
        };

        Users.Add(user);

        return Task.FromResult(_tokenService.GenerateToken(user));
    }

    public Task<string> LoginAsync(LoginRequest request)
    {
        var user = Users.FirstOrDefault(
            existingUser => existingUser.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)
        );

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return Task.FromResult(_tokenService.GenerateToken(user));
    }
}
