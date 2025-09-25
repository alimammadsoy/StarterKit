﻿using MediatR;
using StarterKit.Application.Consts;

namespace StarterKit.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandRequest : IRequest<ResponseDto>
    {
        public string NameSurname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
    }
}