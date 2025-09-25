using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Enums;
using StarterKit.Application.Features.Command.AppUser.AssignRoleToUser;
using StarterKit.Application.Features.Commands.AppUser.CreateUser;
using StarterKit.Application.Features.Commands.AppUser.UpdatePassword;

namespace StarterKit.WebApi.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        [HttpPost]
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

       /* [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get All Users", Menu = "Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest getAllUsersQueryRequest)
        {
            GetAllUsersQueryResponse response = await Mediator.Send(getAllUsersQueryRequest);
            return Ok(response);
        }

        [HttpGet("get-roles-to-user/{UserId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Get Roles To Users", Menu = "Users")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest getRolesToUserQueryRequest)
        {
            GetRolesToUserQueryResponse response = await Mediator.Send(getRolesToUserQueryRequest);
            return Ok(response);
        }*/

        [HttpPost("assign-role-to-user")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "Assign Role To User", Menu = "Users")]
        public async Task<IActionResult> AssignRoleToUser(AssignRoleToUserCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
