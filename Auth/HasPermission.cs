using Microsoft.AspNetCore.Authorization;
using System;

namespace Maucaro.Auth.IdentityPlatform
{
  public class HasPermission : IAuthorizationRequirement
  {
    public string Permission { get; }
    public HasPermission(string permission)
    {
      Permission = permission ?? throw new ArgumentNullException(nameof(permission));
    }
  }
}
