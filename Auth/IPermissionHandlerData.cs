using System.Collections.Generic;

namespace Vida.Prueba.Auth
{
  public interface IPermissionHandlerData
  {
    Dictionary<string, Dictionary<string, HashSet<string>>> GetPermissionRoles();
  }

  public interface IPermissionHandlerDataOptions
  {

  }
}
