
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TSP_LAB1.Models;

namespace TSP_LAB1.Security;

public sealed class OrderOwnerHandler : AuthorizationHandler<OrderOwnerRequirement, CustomerOrder>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OrderOwnerRequirement requirement, CustomerOrder resource)
    {
        // TODO: Osigurati da korisnik iz konteksta može pristupiti svim narudžbama samo ako je administrator.


        // Identitet korisnika dolazi iz validiranog JWT-a.
        var sub = context.User.FindFirstValue("sub");

        if (!Guid.TryParse(sub, out var userId))
            return Task.CompletedTask;

        // TODO: U suprotnom, osigurati da korisnik može pristupiti samo vlastitim narudzbama.


        return Task.CompletedTask;
    }
}
