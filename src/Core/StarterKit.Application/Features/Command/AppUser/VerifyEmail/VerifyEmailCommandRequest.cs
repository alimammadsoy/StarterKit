using MediatR;
using StarterKit.Application.Consts;
using System.Text.Json.Serialization;

namespace StarterKit.Application.Features.Command.AppUser.VerifyEmail
{
    public class VerifyEmailCommandRequest : IRequest<ResponseDto>
    {
        public string Email { get; set; }
        public int VerificationCode { get; set; }
    }
}