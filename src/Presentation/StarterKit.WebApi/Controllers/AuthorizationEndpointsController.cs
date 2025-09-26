﻿using Microsoft.AspNetCore.Mvc;
using StarterKit.Application.Features.Command.AuthorizationEndpoint.AssignRoleEndpoint;
using StarterKit.Application.Features.Queries.AuthorizationEndpoint.GetRolesToEndpoint;

namespace StarterKit.WebApi.Controllers
{
    public class AuthorizationEndpointsController : BaseApiController
    {
        [HttpPost("get-roles-to-endpoint")]
        public async Task<IActionResult> GetRolesToEndpoint(GetRolesToEndpointQueryRequest request)
        {
            var response = await Mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleEndpoint(AssignRoleEndpointCommandRequest request)
        {
            request.Type = typeof(Program);
            var response = await Mediator.Send(request);
            return Ok(response);
        }
    }
}
