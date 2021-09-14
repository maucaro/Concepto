using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace Vida.Prueba.Auth
{
  public class PermissionHandler : AuthorizationHandler<HasPermission>
  {
    private readonly ILogger _logger;
    private readonly IPermissionHandlerData _permissionHandlerData;


    public PermissionHandler(IPermissionHandlerData permissionHandlerData, ILogger<PermissionHandler> logger)
    {
      _logger = logger;
      _permissionHandlerData = permissionHandlerData;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermission requirement)
    {
      string tenant = context.User.Claims.Where(c => c.Type == CustomAuthenticationDefaults.TenantClaim).Select(c => c.Value).FirstOrDefault();
      HashSet<string> userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet();
      if (_permissionHandlerData.HasPermission(tenant, requirement.Permission, userRoles))
      {
        context.Succeed(requirement);
      }
      return Task.CompletedTask;
    }
  }
}
