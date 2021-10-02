using System;

namespace Maucaro.Auth.IdentityPlatform
{
  public class PermissionHandlerSqlOptions
  {
    private string _connectionString;
    private string _permissionRolesStoredProcedure;
    private string _tenantField;
    private string _permissionField;
    private string _roleField;
    private int _intervalSeconds;

    public string ConnectionString
    {
      get
      {
        return _connectionString;
      }
      set
      {
        _connectionString = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof(ConnectionString));
      }
    }
    public string PermissionRolesStoredProcedure
    {
      get
      {
        return _permissionRolesStoredProcedure;
      }
      set
      {
        _permissionRolesStoredProcedure = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof(PermissionRolesStoredProcedure));
      }
    }
    public string TenantField
    {
      get
      {
        return _tenantField;
      }
      set
      {
        _tenantField = !string.IsNullOrWhiteSpace(value) ? value : "Tenant";
      }
    }
    public string PermissionField
    {
      get
      {
        return _permissionField;
      }
      set
      {
        _permissionField = !string.IsNullOrWhiteSpace(value) ? value : "Permission";
      }
    }

    public string RoleField
    {
      get
      {
        return _roleField;
      }
      set
      {
        _roleField = !string.IsNullOrWhiteSpace(value) ? value : "Role";
      }
    }

    public int IntervalSeconds
    {
      get
      {
        return _intervalSeconds;
      }
      set
      {
        _intervalSeconds = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(IntervalSeconds));
      }
    }
  }
}