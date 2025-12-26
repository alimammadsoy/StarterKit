using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Enums;
using StarterKit.Application.Features.Commands.Role.CreateRole;
using StarterKit.Application.Features.Commands.Role.DeleteRole;
using StarterKit.Application.Features.Commands.Role.UpdateRole;
using StarterKit.Application.Features.Queries.Role.GetRoleById;
using StarterKit.Application.Features.Queries.Role.GetRoles;

namespace StarterKit.WebApi.Controllers
{
    [Authorize]
    public class RolesController : BaseApiController
    {

        [HttpGet]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "roles.views", Menu = "Roles")]
        public async Task<IActionResult> GetRoles([FromQuery] GetRolesQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "roles.view", Menu = "Roles")]
        public async Task<IActionResult> GetRoleById([FromRoute] GetRoleByIdQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost()]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "roles.create", Menu = "Roles")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return StatusCode(StatusCodes.Status201Created, response);
            //return Ok(response);
        }

        [HttpPut("{id}")]
        [AuthorizeDefinition(ActionType = ActionType.Updating, Definition = "roles.update", Menu = "Roles")]
        public async Task<IActionResult> UpdateRole([FromRoute] int id, [FromBody] UpdateRoleCommandRequest request)
        {
            request.Id = id;
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("{Id}")]
        [AuthorizeDefinition(ActionType = ActionType.Deleting, Definition = "roles.delete", Menu = "Roles")]
        public async Task<IActionResult> DeleteRole([FromRoute] DeleteRoleCommandRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
