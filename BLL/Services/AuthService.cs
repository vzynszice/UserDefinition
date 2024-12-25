using DAL.Models;
using DAL.db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BLL.interfaces;

namespace BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly HaosDbContext _context;

        public AuthService(HaosDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<AuthResult> LoginAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null)
            {
                return new AuthResult { Succeeded = false, ErrorMessage = "User not found." };
            }
            if (user.Password != password)
            {
                return new AuthResult { Succeeded = false, ErrorMessage = "Invalid password." };
            }
            return new AuthResult { Succeeded = true , User=user};
        }
    }

    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
        public User User { get; set;  }
    }
}
