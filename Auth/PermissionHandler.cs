using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
      HashSet<string> permissionRoles = _permissionHandlerData.GetPermissionRoles().GetValueOrDefault(tenant)?.GetValueOrDefault(requirement.Permission);
      if (permissionRoles != null && permissionRoles.Overlaps(userRoles))
      { 
        context.Succeed(requirement);
      }
      return Task.CompletedTask;
    }
  }
}
