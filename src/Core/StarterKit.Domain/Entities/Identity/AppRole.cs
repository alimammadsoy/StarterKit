﻿using Microsoft.AspNetCore.Identity;
using StarterKit.Domain.Entities.EndpointAggregate;
using System.Net;

namespace StarterKit.Domain.Entities.Identity
{
    public class AppRole : IdentityRole<string>
    {
        public ICollection<Endpoint> Endpoints { get; set; }
    }
}
