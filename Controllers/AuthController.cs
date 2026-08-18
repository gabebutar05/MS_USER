using MediatR;
using Microsoft.AspNetCore.Mvc;
using MS_USER.Features.Login;

namespace MS_USER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISender _sender;

        public AuthController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Login user dengan username dan password.
        /// Semua logic diproses di LoginHandler via MediatR.
        /// </summary>
        /// <param name="command">Data login (username & password)</param>
        /// <returns>Token jika berhasil, pesan error jika gagal</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await _sender.Send(command);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(result);
        }
    }
}
