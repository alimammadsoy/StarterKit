using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.Features.Command.AppUser.LoginUser;
using StarterKit.Application.Features.Command.AppUser.PasswordReset;
using StarterKit.Application.Features.Command.AppUser.RefreshToken;
using StarterKit.Application.Features.Command.AppUser.VerifyResetToken;

namespace StarterKit.WebApi.Controllers
{
    public class AuthController : BaseApiController
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommandRequest passwordResetCommandRequest)
        {
            var response = await Mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }
        
        [HttpPost("verify-reset-token")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        /*[HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginCommandRequest googleLoginCommandRequest)
        {
            GoogleLoginCommandResponse response = await _mediator.Send(googleLoginCommandRequest);
            return Ok(response);
        }*/

        /*[HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin(FacebookLoginCommandRequest facebookLoginCommandRequest)
        {
            FacebookLoginCommandResponse response = await _mediator.Send(facebookLoginCommandRequest);
            return Ok(response);
        }*/
    }
}
