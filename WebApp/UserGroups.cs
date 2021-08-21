using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Timers;

namespace Vida.Prueba.WebApp
{
  public interface IUserGroups
  {
    public IReadOnlyList<string> GetGroups(string email);
  }
  public class UserGroups : IUserGroups
  {
    private Dictionary<string, List<string>> _userGroups;
    private string _connectionString;
    private string _storedProcedure;
    private string _emailField;
    private string _groupField;
    private Timer _timer;
    public UserGroups(IConfiguration configuration)
    {
      _connectionString = configuration.GetSection("DbUsers").GetValue<string>("ConnectionString");
      _storedProcedure = configuration.GetSection("DbUsers").GetValue<string>("StoredProcedure");
      _emailField = configuration.GetSection("DbUsers").GetValue<string>("EmailField");
      _groupField = configuration.GetSection("DbUsers").GetValue<string>("GroupField");
      var interval = configuration.GetSection("DbUsers").GetValue<int>("Interval");
      this.UpdateUserGroups();
      _timer = new System.Timers.Timer();
      _timer.Interval = interval;
      _timer.Elapsed += this.UpdateUserGroups;
      _timer.AutoReset = true;
      _timer.Enabled = true;
    }

    public void UpdateUserGroups(Object source, System.Timers.ElapsedEventArgs e)
    {
      UpdateUserGroups();
    }
    public void UpdateUserGroups()
    {
      Dictionary<string, List<string>> userGroups = new();
      using (SqlConnection connection = new SqlConnection(_connectionString))
      {
        connection.Open();
        SqlCommand command = new();
        command.Connection = connection;
        command.CommandText = _storedProcedure;
        command.CommandType = System.Data.CommandType.StoredProcedure;
        using (SqlDataReader reader = command.ExecuteReader())
        {
          if (reader.HasRows)
          {
            while (reader.Read())
            {
              var email = (string)reader[_emailField];
              var group = (string)reader[_groupField];
              if (userGroups.ContainsKey(email))
              {
                userGroups.GetValueOrDefault(email).Add(group);
              }
              else
              {
                List<string> groups = new();
                groups.Add(group);
                userGroups.Add(email, groups);
              }

            }
          }
          reader.Close();
        }
      }
      _userGroups = userGroups;
    }

    public IReadOnlyList<string> GetGroups(string email)
    {
      return _userGroups.GetValueOrDefault(email).AsReadOnly();
    }
  }
}
