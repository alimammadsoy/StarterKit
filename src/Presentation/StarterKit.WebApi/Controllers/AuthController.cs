using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.Features.Command.AppUser.LoginUser;

namespace StarterKit.WebApi.Controllers
{
    public class AuthController : BaseApiController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

       /* [HttpPost("[action]")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest refreshTokenLoginCommandRequest)
        {
            RefreshTokenLoginCommandResponse response = await _mediator.Send(refreshTokenLoginCommandRequest);
            return Ok(response);
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommandRequest passwordResetCommandRequest)
        {
            PasswordResetCommandResponse response = await _mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }

        [HttpPost("verify-reset-token")]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenCommandRequest verifyResetTokenCommandRequest)
        {
            VerifyResetTokenCommandResponse response = await _mediator.Send(verifyResetTokenCommandRequest);
            return Ok(response);
        }*/
    }
}
