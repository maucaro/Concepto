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
    private Dictionary<string, List<string>> _permissionGroups;
    private readonly string _connectionString;
    private readonly string _storedProcedure;
    private readonly string _permissionField;
    private readonly string _groupField;
    private readonly Timer _timer;


    public PermissionHandler(IConfiguration configuration)
    {
      _connectionString = configuration.GetSection("DbUsers").GetValue<string>("ConnectionString");
      _storedProcedure = configuration.GetSection("DbUsers").GetValue<string>("PermissionGroupsSP");
      _permissionField = configuration.GetSection("DbUsers").GetValue<string>("PermissionField");
      _groupField = configuration.GetSection("DbUsers").GetValue<string>("GroupField");
      var interval = configuration.GetSection("DbUsers").GetValue<int>("Interval");
      this.UpdatePermissionGroups();
      _timer = new System.Timers.Timer
      {
        Interval = interval
      };
      _timer.Elapsed += this.UpdatePermissionGroups;
      _timer.AutoReset = true;
      _timer.Enabled = true;
    }

    private void UpdatePermissionGroups(Object source, System.Timers.ElapsedEventArgs e)
    {
      this.UpdatePermissionGroups();
    }

    private void UpdatePermissionGroups()
    {
      Dictionary<string, List<string>> permissionGroups = new();
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
            var group = (string)reader[_groupField];
            if (permissionGroups.ContainsKey(permission))
            {
              permissionGroups.GetValueOrDefault(permission).Add(group);
            }
            else
            {
              List<string> groups = new();
              groups.Add(group);
              permissionGroups.Add(permission, groups);
            }

          }
        }
        reader.Close();
      }
      _permissionGroups = permissionGroups;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermission requirement)
    {
      if (_permissionGroups.ContainsKey(requirement.Permission) && context.User.Claims.Count() > 0)
      {
        List<string> permissionGroups = _permissionGroups.GetValueOrDefault(requirement.Permission);
        List<string> userGroups = context.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        foreach (string group in userGroups)
        {
          if (permissionGroups.Contains(group))
          {
            context.Succeed(requirement);
            break;
          }
        }
      }
      return Task.CompletedTask;
    }
  }
}
