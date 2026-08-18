namespace MS_USER.Features.Login
{
    /// <summary>
    /// Response yang dikembalikan oleh LoginHandler setelah proses autentikasi.
    /// </summary>
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
    }
}
