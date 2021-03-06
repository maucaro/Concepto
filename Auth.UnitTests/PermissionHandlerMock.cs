using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maucaro.Auth.IdentityPlatform.UnitTests
{
  class PermissionHandlerMock : IPermissionHandlerData
  {
    private readonly string _tenant = "maucaro";
    private readonly string _role1 = "role1";
    private readonly string _role2 = "role2";
    private readonly string _permission1 = "perm1";
    private readonly string _permission2 = "perm2";
    private readonly string _permission3 = "perm3";

    private Dictionary<string, Dictionary<string, HashSet<string>>> _permissionRoles = new();

    public PermissionHandlerMock()
    {
      var roles1 = new HashSet<string> { _role1 };
      var roles2 = new HashSet<string> { _role2 };

      var permRoles = new Dictionary<string, HashSet<string>> { { _permission1, roles1 }, { _permission2, roles2 }, { _permission3, roles2 } };

      _permissionRoles.Add(_tenant, permRoles);
    }
    public IEnumerable<string> GetRoles(string tenant, string permission)
    {
      return _permissionRoles?.GetValueOrDefault(tenant ?? string.Empty)?.GetValueOrDefault(permission ?? string.Empty);
    }

    public void SetPermissionRoles(Dictionary<string, Dictionary<string, HashSet<string>>> permissionRoles)
    {
      _permissionRoles = permissionRoles;
    }
  }
}
