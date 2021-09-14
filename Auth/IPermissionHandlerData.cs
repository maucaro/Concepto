using System.Collections.Generic;

namespace Vida.Prueba.Auth
{
  public interface IPermissionHandlerData
  {
    public bool HasPermission(string tenant, string permission, HashSet<string> userRoles);
  }

  public interface IPermissionHandlerDataOptions
  {

  }
}
