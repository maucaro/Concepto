using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vida.Prueba.Auth.UnitTests
{
  [TestClass]
  public class UnitTestsPermissionHandler
  {
    private const string sub = "sub";
    private const string email = "test@maucaro.com";
    private const string tenant = "maucaro";
    private readonly AuthorizationHandler<HasPermission> _permissionHandler;

    public UnitTestsPermissionHandler()
    {
      // PermissionHandlerMock has following tenant, permission, role mappings:
      // maucaro -> perm1 -> role1
      // maucaro -> perm2 -> role2, role3
      PermissionHandlerMock permissionHandlerMock = new();
      _permissionHandler = new PermissionHandler(permissionHandlerMock);
    }

    private async Task HasPermissionHelper(string tenant, string[] roles, string permission, bool success, PermissionHandler permissionHandler = null)
    {
      if (permissionHandler == null)
      {
        permissionHandler = (PermissionHandler)_permissionHandler;
      }
      List<Claim> claims = new()
      {
        new Claim(ClaimTypes.NameIdentifier, sub),
        new Claim(ClaimTypes.Email, email)
      };
      if (tenant != null)
      {
        claims.Add(new Claim(CustomAuthenticationDefaults.TenantClaim, tenant));
      }
      foreach (string role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
      var claimsIdentity = new ClaimsIdentity(claims, nameof(ValidateAuthenticationHandler));
      var user = new ClaimsPrincipal(claimsIdentity);
      var requirement = new HasPermission(permission);
      var requirements = new List<HasPermission> { requirement };
      var context = new DefaultHttpContext();
      var handlerContext = new AuthorizationHandlerContext(requirements, user, context);
      await permissionHandler.HandleAsync(handlerContext);
      if (success)
      {
        Assert.IsTrue(handlerContext.HasSucceeded);
      }
      else
      {
        Assert.IsFalse(handlerContext.HasSucceeded);
      }

    }

    [TestMethod]
    public async Task Test_HasPermission()
    {
      await HasPermissionHelper(tenant, new string[] { "role1" }, "perm1", true);
    }
    [TestMethod]
    public async Task Test_HasPermissionTwoRoles()
    {
      await HasPermissionHelper(tenant, new string[] { "role1", "role3" }, "perm1", true);
    }
    [TestMethod]
    public async Task Test_InexistentRole()
    {
      await HasPermissionHelper(tenant, new string[] { "role3" }, "perm1", false);
    }
    [TestMethod]
    public async Task Test_NoRole()
    {
      await HasPermissionHelper(tenant, System.Array.Empty<string>(), "perm1", false);
    }
    [TestMethod]
    public async Task Test_NoPermission()
    {
      await HasPermissionHelper(tenant, new string[] { "role2" }, "perm1", false);
    }
    [TestMethod]
    public async Task Test_InexistentTenant()
    {
      await HasPermissionHelper("faketenant", new string[] { "role1" }, "perm1", false);
    }
    [TestMethod]
    public async Task Test_NoTenant()
    {
      await HasPermissionHelper(null, new string[] { "role1" }, "perm1", false);
    }

    [TestMethod]
    public async Task Test_EmptyPermissionHandlerData()
    {
      PermissionHandlerMock permissionHandlerMock = new();
      permissionHandlerMock.SetPermissionRoles(new Dictionary<string, Dictionary<string, HashSet<string>>>());
      var permissionHandler = new PermissionHandler(permissionHandlerMock);
      await HasPermissionHelper(tenant, new string[] { "role1" }, "perm1", false, permissionHandler);
    }
    [TestMethod]
    public async Task Test_NullPermissionHandlerData()
    {
      PermissionHandlerMock permissionHandlerMock = new();
      Dictionary<string, Dictionary<string, HashSet<string>>> nullData = null;
      permissionHandlerMock.SetPermissionRoles(nullData);
      var permissionHandler = new PermissionHandler(permissionHandlerMock);
      await HasPermissionHelper(tenant, new string[] { "role1" }, "perm1", false, permissionHandler);
    }
  }
}
