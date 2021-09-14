using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Timers;

namespace Vida.Prueba.Auth
{
  public class PermissionHandlerSql : IPermissionHandlerData
  {
    private readonly ILogger _logger;
    private Dictionary<string, Dictionary<string, HashSet<string>>> _permissionRoles = new();
    private readonly PermissionHandlerSqlOptions _options;
    private readonly Timer _timer;
    public PermissionHandlerSql(IOptions<PermissionHandlerSqlOptions> options, ILogger<PermissionHandlerSql> logger)
    {
      _options = options.Value;
      _logger = logger;
      this.UpdatePermissionRoles();
      _timer = new Timer
      {
        Interval = _options.IntervalSeconds * 1000
      };
      _timer.Elapsed += this.UpdatePermissionRoles;
      _timer.AutoReset = true;
      _timer.Enabled = true;
    }
    public bool HasPermission(string tenant, string permission, HashSet<string> userRoles)
    {
      HashSet<string> permissionRoles = _permissionRoles.GetValueOrDefault(tenant).GetValueOrDefault(permission);
      return permissionRoles.Overlaps(userRoles);
    }

    private void UpdatePermissionRoles(Object source, System.Timers.ElapsedEventArgs e)
    {
      this.UpdatePermissionRoles();
    }

    private void UpdatePermissionRoles()
    {
      Dictionary<string, Dictionary<string, HashSet<string>>> permissionRoles = new();
      using SqlConnection connection = new(_options.ConnectionString);
      connection.Open();
      SqlCommand command = new();
      command.Connection = connection;
      command.CommandText = _options.PermissionRolesStoredProcedure;
      command.CommandType = System.Data.CommandType.StoredProcedure;
      try
      {
        using SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            var tenant = (string)reader[_options.TenantField] ?? String.Empty;
            var permission = (string)reader[_options.PermissionField];
            var role = (string)reader[_options.RoleField];
            Dictionary<string, HashSet<string>> permissionRolesTenant;
            if (!permissionRoles.ContainsKey(tenant))
            {
              permissionRolesTenant = new();
              permissionRoles.Add(tenant, permissionRolesTenant);
            }
            else
            {
              permissionRolesTenant = permissionRoles.GetValueOrDefault(tenant);
            }
            if (permissionRolesTenant.ContainsKey(permission))
            {
              permissionRolesTenant.GetValueOrDefault(permission).Add(role);
            }
            else
            {
              HashSet<string> roles = new() { role };
              permissionRolesTenant.Add(permission, roles);
            }

          }
        }
        reader.Close();
        _logger.LogInformation("Permissions and Roles refreshed");
        _permissionRoles = permissionRoles;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error refreshing permissions and roles");
      }
    }
  }
}
