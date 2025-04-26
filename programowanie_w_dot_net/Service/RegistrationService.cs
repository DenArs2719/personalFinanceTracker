using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Service;

using Microsoft.AspNetCore.Identity;

public class RegistrationService
{
    private readonly BudgetDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public RegistrationService(BudgetDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public void CreateUser(string login, string password)
    {
        var user = new User
        {
            Login = login,
            PasswordHash = _passwordHasher.HashPassword(null, password) // Hash the password
        };

        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result == PasswordVerificationResult.Success;
    }
}
