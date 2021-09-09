using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace Vida.Prueba.Auth
{
  public class PermissionHandler : AuthorizationHandler<HasPermission>
  {
    private const string TenantClaim = "tenant";
    private readonly ILogger _logger;
    private Dictionary<string, Dictionary<string, HashSet<string>>> _permissionRoles = new();
    private readonly string _connectionString;
    private readonly string _storedProcedure;
    private readonly string _tenantField;
    private readonly string _permissionField;
    private readonly string _roleField;
    private readonly Timer _timer;
    private readonly int _intervalSeconds;


    public PermissionHandler(IConfiguration configuration, ILogger<PermissionHandler> logger)
    {
      _logger = logger;
      _connectionString = configuration.GetSection("DbUsers").GetValue<string>("ConnectionString");
      _storedProcedure = configuration.GetSection("DbUsers").GetValue<string>("PermissionRolesSP");
      _tenantField = configuration.GetSection("DbUsers").GetValue<string>("TenantField");
      _permissionField = configuration.GetSection("DbUsers").GetValue<string>("PermissionField");
      _roleField = configuration.GetSection("DbUsers").GetValue<string>("RoleField");
      _intervalSeconds = configuration.GetSection("DbUsers").GetValue<int>("IntervalSeconds");
      this.UpdatePermissionRoles();
      _timer = new System.Timers.Timer
      {
        Interval = _intervalSeconds * 1000
      };
      _timer.Elapsed += this.UpdatePermissionRoles;
      _timer.AutoReset = true;
      _timer.Enabled = true;
    }

    private void UpdatePermissionRoles(Object source, System.Timers.ElapsedEventArgs e)
    {
      this.UpdatePermissionRoles();
    }

    private void UpdatePermissionRoles()
    {
      Dictionary<string, Dictionary<string, HashSet<string>>> permissionRoles = new();
      using (SqlConnection connection = new(_connectionString))
      {
        connection.Open();
        SqlCommand command = new();
        command.Connection = connection;
        command.CommandText = _storedProcedure;
        command.CommandType = System.Data.CommandType.StoredProcedure;
        try
        {
          using SqlDataReader reader = command.ExecuteReader();
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              var tenant = (string)reader[_tenantField] ?? String.Empty;
              var permission = (string)reader[_permissionField];
              var role = (string)reader[_roleField];
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

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermission requirement)
    {
      string tenant = context.User.Claims.Where(c => c.Type == TenantClaim).Select(c => c.Value).FirstOrDefault();
      if (_permissionRoles.ContainsKey(tenant) && context.User.Claims.Any())
      {
        HashSet<string> permissionRoles = _permissionRoles.GetValueOrDefault(tenant).GetValueOrDefault(requirement.Permission);
        HashSet<string> userRoles = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet();
        if (permissionRoles.Overlaps(userRoles))
        {
          context.Succeed(requirement);
        }
      }
      return Task.CompletedTask;
    }
  }
}
