using System.Collections.Generic;

namespace Vida.Prueba.Auth
{
  public interface IPermissionHandlerData
  {
    IEnumerable<string> GetRoles(string tenant, string permission);
  }

  public interface IPermissionHandlerDataOptions
  {

  }
}
