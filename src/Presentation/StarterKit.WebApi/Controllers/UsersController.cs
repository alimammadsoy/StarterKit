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

namespace StarterKit.WebApi.Controllers
{
    public class UsersController : BaseApiController
    {
        [HttpPost]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Create Users", Menu = "Users")]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            var response = await Mediator.Send(updatePasswordCommandRequest);
            return Ok(response);
        }

        [HttpGet]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get All Users", Menu = "Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get User By Id", Menu = "Users")]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetUserByIdQueryRequest(id));
            return Ok(response);
        }

        [HttpPut("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "Update User", Menu = "Users")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserCommandRequest request)
        {
            request.Id = id;
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "Delete User", Menu = "Users")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var response = await Mediator.Send(new DeleteUserCommandRequest(id));
            return Ok(response);
        }
    }
}
