using MediatR;
using Microsoft.EntityFrameworkCore;
using MS_USER.Data;

namespace MS_USER.Features.Login
{
    /// <summary>
    /// Handler yang memproses LoginCommand.
    /// Semua business logic autentikasi berada di sini.
    /// </summary>
    public class LoginHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly AppDbContext _context;

        public LoginHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            // Cari user berdasarkan username, hanya yang aktif (RowStatus = 1)
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                    u.Username == command.Username &&
                    u.RowStatus >= 0,
                    cancellationToken);

            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Username atau password salah"
                };
            }

            // TODO: Ganti dengan BCrypt.Verify() jika password di-hash
            // Saat ini: plain text comparison
            if (user.Password != command.Password)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Username atau password salah"
                };
            }

            // TODO: Generate JWT token yang sesungguhnya
            var token = GenerateDummyToken(user.Username, user.RoleId);

            return new LoginResponse
            {
                Success = true,
                Message = "Login berhasil",
                Token = token
            };
        }

        /// <summary>
        /// Temporary — akan diganti dengan JWT generation sesungguhnya.
        /// </summary>
        private static string GenerateDummyToken(string username, int roleId)
        {
            return $"dummy-token|user:{username}|role:{roleId}|{Guid.NewGuid()}";
        }
    }
}
