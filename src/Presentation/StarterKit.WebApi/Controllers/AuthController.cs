using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.Features.Command.AppUser.ForgotPassword;
using StarterKit.Application.Features.Command.AppUser.LoginUser;
using StarterKit.Application.Features.Command.AppUser.LogoutUser;
using StarterKit.Application.Features.Command.AppUser.PasswordReset;
using StarterKit.Application.Features.Command.AppUser.RefreshToken;
using StarterKit.Application.Features.Command.AppUser.RegisterUser;
using StarterKit.Application.Features.Command.AppUser.ResendResetCode;
using StarterKit.Application.Features.Command.AppUser.ResendVerificationCode;
using StarterKit.Application.Features.Command.AppUser.ResetPassword;
using StarterKit.Application.Features.Command.AppUser.VerifyEmail;
using StarterKit.Application.Features.Command.AppUser.VerifyResetCode;
using StarterKit.Application.Features.Queries.Auth;
using StarterKit.WebApi.Attributes;

namespace StarterKit.WebApi.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseApiController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("register")]
        [LocalizeResponse]
        public async Task<IActionResult> Register(RegisterUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
            //return Ok(response);
        }

        [HttpPost("verify-email")]
        [LocalizeResponse]
        public async Task<IActionResult> VerifyEmail(VerifyEmailCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
            //return Ok(response);
        }

        [HttpPost("resend-verification-code")]
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenLogin([FromBody] RefreshTokenLoginCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        [LocalizeResponse]
        public async Task<IActionResult> Logout([FromBody] LogoutCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        [LocalizeResponse]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("verify-reset-code")]
        [LocalizeResponse]
        public async Task<IActionResult> VerifyResetCode(VerifyResetCodeCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("resend-reset-code")]
        [LocalizeResponse]
        public async Task<IActionResult> ResendResetCode(ResendResetCodeCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        [LocalizeResponse]
        public async Task<IActionResult> PasswordReset([FromBody] ResetPasswordCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

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
