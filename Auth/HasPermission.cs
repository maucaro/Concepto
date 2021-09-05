using Microsoft.AspNetCore.Authorization;
using System;

namespace Vida.Prueba.Auth
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
