using System.Collections.Generic;

namespace Maucaro.Auth.IdentityPlatform
{
  public interface IPermissionHandlerData
  {
    IEnumerable<string> GetRoles(string tenant, string permission);
  }

  public interface IPermissionHandlerDataOptions
  {

  }
}
