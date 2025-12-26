using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Enums;
using StarterKit.Application.Features.Command.AppUser.CreateUser;
using StarterKit.Application.Features.Command.AppUser.DeleteUser;
using StarterKit.Application.Features.Command.AppUser.UpdateUser;
using StarterKit.Application.Features.Commands.AppUser.UpdatePassword;
using StarterKit.Application.Features.Queries.User.GetAllUsers;
using StarterKit.Application.Features.Queries.User.GetById;
using StarterKit.WebApi.Attributes;

namespace StarterKit.WebApi.Controllers
{
    public class UsersController : BaseApiController
    {
        [HttpPost]
        [Authorize]
        [LocalizeResponse]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "users.create", Menu = "Users")]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
            //return Ok(response);
        }

        [LocalizeResponse]
        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            var response = await Mediator.Send(updatePasswordCommandRequest);
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "users.views", Menu = "Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "users.view", Menu = "Users")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetUserByIdQueryRequest(id));
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "users.update", Menu = "Users")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserCommandRequest request)
        {
            request.Id = id;
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "users.delete", Menu = "Users")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteUserCommandRequest(id));
            return Ok(response);
        }
    }
}
