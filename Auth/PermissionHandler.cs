using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Maucaro.Auth.IdentityPlatform
{
  public class PermissionHandler : AuthorizationHandler<HasPermission>
  {
    private readonly IPermissionHandlerData _permissionHandlerData;


    public PermissionHandler(IPermissionHandlerData permissionHandlerData)
    {
      _permissionHandlerData = permissionHandlerData;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermission requirement)
    {
      string tenant = context.User.Claims.Where(c => c.Type == CustomAuthenticationDefaults.TenantClaim).Select(c => c.Value).FirstOrDefault() ?? string.Empty;
      HashSet<string> userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet();
      IEnumerable<string> permissionRoles = _permissionHandlerData.GetRoles(tenant, requirement.Permission);
      if (userRoles != null && permissionRoles != null && userRoles.Overlaps(permissionRoles))
      {
        context.Succeed(requirement);
      }
      return Task.CompletedTask;
    }
  }
}
