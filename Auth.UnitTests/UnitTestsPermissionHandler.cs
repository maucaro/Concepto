using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
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
    private const string scheme = "default";
    private readonly Mock<ILogger<PermissionHandler>> _logger;
    private readonly AuthorizationHandler<HasPermission> _permissionHandler;

    public UnitTestsPermissionHandler()
    {
      _logger = new Mock<ILogger<PermissionHandler>>();
      // PermissionHandlerMock has following tenant, permission, role mappings:
      // maucaro -> perm1 -> role1
      // maucaro -> perm2 -> role2, role3
      PermissionHandlerMock permissionHandlerMock = new();
      _permissionHandler = new PermissionHandler(permissionHandlerMock, _logger.Object);
    }

    [TestMethod]
    public async Task Test_HasPermission()
    {

      List<Claim> claims = new()
      {
        new Claim(ClaimTypes.NameIdentifier, sub),
        new Claim(ClaimTypes.Email, email),
        new Claim(CustomAuthenticationDefaults.TenantClaim, tenant)
      };

      List<string> roles = new() { "role1" };
      foreach (string role in roles)
      {
        claims.Add(new Claim(ClaimTypes.Role, role));
      }
      var claimsIdentity = new ClaimsIdentity(claims, nameof(ValidateAuthenticationHandler));
      var user = new ClaimsPrincipal(claimsIdentity);
      var requirement = new HasPermission("perm1");
      var requirements = new List<HasPermission> { requirement };
      var context = new DefaultHttpContext();
      var handlerContext = new AuthorizationHandlerContext(requirements, user, context);
      await _permissionHandler.HandleAsync(handlerContext);
      Assert.IsTrue(handlerContext.HasSucceeded);
    }
  }
}
