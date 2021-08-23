using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;

namespace Vida.Prueba.WebApp
{
  public class PermissionHandler : AuthorizationHandler<HasPermission>
  {
    private Dictionary<string, HashSet<string>> _permissionRoles;
    private readonly string _connectionString;
    private readonly string _storedProcedure;
    private readonly string _permissionField;
    private readonly string _roleField;
    private readonly Timer _timer;


    public PermissionHandler(IConfiguration configuration)
    {
      _connectionString = configuration.GetSection("DbUsers").GetValue<string>("ConnectionString");
      _storedProcedure = configuration.GetSection("DbUsers").GetValue<string>("PermissionRolesSP");
      _permissionField = configuration.GetSection("DbUsers").GetValue<string>("PermissionField");
      _roleField = configuration.GetSection("DbUsers").GetValue<string>("RoleField");
      var interval = configuration.GetSection("DbUsers").GetValue<int>("Interval");
      this.UpdatePermissionRoles();
      _timer = new System.Timers.Timer
      {
        Interval = interval
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
      Dictionary<string, HashSet<string>> permissionRoles = new();
      using (SqlConnection connection = new(_connectionString))
      {
        connection.Open();
        SqlCommand command = new();
        command.Connection = connection;
        command.CommandText = _storedProcedure;
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using SqlDataReader reader = command.ExecuteReader();
        if (reader.HasRows)
        {
          while (reader.Read())
          {
            var permission = (string)reader[_permissionField];
            var role = (string)reader[_roleField];
            if (permissionRoles.ContainsKey(permission))
            {
              permissionRoles.GetValueOrDefault(permission).Add(role);
            }
            else
            {
              HashSet<string> roles = new() { role };
              permissionRoles.Add(permission, roles);
            }

          }
        }
        reader.Close();
      }
      _permissionRoles = permissionRoles;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermission requirement)
    {
      if (_permissionRoles.ContainsKey(requirement.Permission) && context.User.Claims.Any())
      {
        HashSet<string> permissionRoles = _permissionRoles.GetValueOrDefault(requirement.Permission);
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
