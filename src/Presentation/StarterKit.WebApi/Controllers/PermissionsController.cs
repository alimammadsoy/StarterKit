using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.CustomAttributes;
using StarterKit.Application.Enums;
using StarterKit.Application.Features.Command.AuthorizationEndpoint.SyncEndpoints;
using StarterKit.Application.Features.Queries.Endpoint.GetAll;
using StarterKit.Application.Features.Queries.Endpoint.GetById;

namespace StarterKit.WebApi.Controllers
{
    [Authorize]
    public class PermissionsController : BaseApiController
    {
        [HttpPost("sync")]
        [AuthorizeDefinition(ActionType = ActionType.Writing, Definition = "Sync Permissions", Menu = "Permissions")]
        public async Task<IActionResult> SyncPermissions()
        {
            var response = await Mediator.Send(new SyncEndpointsCommandRequest());
            return Ok(response);
        }

        [HttpGet]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "permissions.views", Menu = "Permissions")]
        public async Task<IActionResult> GetPermissions([FromQuery] GetEndpointsQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AuthorizeDefinition(ActionType = ActionType.Reading, Definition = "permissions.view", Menu = "Permissions")]
        public async Task<IActionResult> GetPermissionById([FromRoute] int id)
        {
            var response = await Mediator.Send(new GetEndpointByIdQueryRequest(id));
            return Ok(response);
        }
    }
}
