using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Enums;
using StarterKit.Application.Features.Command.AppUser.LoginUser;
using StarterKit.Application.Features.Command.AppUser.PasswordReset;
using StarterKit.Application.Features.Command.AppUser.RefreshToken;
using StarterKit.Application.Features.Command.AppUser.RegisterUser;
using StarterKit.Application.Features.Command.AppUser.ResendVerificationCode;
using StarterKit.Application.Features.Command.AppUser.VerifyEmail;
using StarterKit.Application.Features.Command.AppUser.VerifyResetToken;
using StarterKit.Application.Features.Queries.Auth;
using StarterKit.WebApi.Attributes;

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

        [HttpPost("register")]
        [AllowAnonymous]
        [LocalizeResponse]
        public async Task<IActionResult> Register(RegisterUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        [LocalizeResponse]
        public async Task<IActionResult> VerifyEmail(VerifyEmailCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("resend-verification-code")]
        [AllowAnonymous]
        [LocalizeResponse]
        public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var response = await Mediator.Send(new GetProfileQueryRequest());
            return Ok(response);
        }

        /*[HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }*/

        /*[HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordReset([FromBody] PasswordResetCommandRequest passwordResetCommandRequest)
        {
            var response = await Mediator.Send(passwordResetCommandRequest);
            return Ok(response);
        }*/

        /*[HttpPost("verify-reset-token")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyResetToken([FromBody] VerifyResetTokenCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }*/

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
