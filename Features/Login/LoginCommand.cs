using MediatR;

namespace MS_USER.Features.Login
{
    /// <summary>
    /// Command yang dikirim dari controller ke handler via MediatR.
    /// Berisi data yang dibutuhkan untuk proses login.
    /// </summary>
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
