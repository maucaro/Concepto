# Maucaro.Auth.IdentityPlatform
This solution is comprised of:
- Auth - Main asset: This library enables using Google Identity Platform and/or Firebase as the IdP in a .Net Core Web or API project. It is published in Nuget.org.
- Auth.UnitTests - Unit tests for the Auth library.
- DbUsers - Sample SQL Server project used to store roles to permissions mappings in support of a Custom Permission Handler in the Auth library
- DbUsers.build - Deployment artifact based on dacpac used to deploy DbUsers
- Deploy - Various artifacts in support of CI/CD. Azure Pipelines are used for the builds and Google Cloud Build is used for deployment to GKE
- WebApp - Sample application that uses the Auth library and the DbUsers database

# Auth
- ValidateAuthenticationHandler is a custom authentication handler (derrives from Microsoft.AspNetCore.Authentication.AuthenticationHandler) that validates the JWT and sets ClaimsPrincipal. It can be configured with ValidateAuthenticationSchemeOptions with:
  - CertificatesUrl: For IdentityPlatform or Firebase, the value should be "https://www.googleapis.com/service_accounts/v1/jwk/securetoken@system.gserviceaccount.com"
  - TrustedAudience: This is the GCP Project ID that hosts Cloud Identity or Firebase
  - ValidTenants: If specified, validates that the JWT tenant claim matches one of the ones specified. If not specified, no validation is made which can be used for either single-instance use cases or for multi-tenant ones where all tenants are allowed.
- The ClaimsPrincipal set by ValidateAuthenticationHandler will have:
  - ClaimTypes.NameIdentifier - corresponds to the 'sub' claim in the JWT
  - ClaimTypes.Email - corresponds to the 'email' claim in the JWT
  - ClaimTypes.Name - corresponds to the 'name' claim in the JWT, if present.
  - Tenant (constant defined in CustomAuthenticationDefaults.TenantClaim) - corresponds to the 'firebase.tenant' claim, if present.
  - ClaimTypes.Role - zero, one or more role claims corresponding to the custom 'role' claim in the JWT in array format. If used, this allows the native .Net Core role-based authorization constructs (https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0)
- PermissionsPolicyProvider is a dynamic policy provider. It requires a policy store that implements IPermissionHandlerData. The policy store maps a permission, on a per-tenant basis, to zero, one or multiple roles. This allows for policy-based role checks ([Authorize(Policy = "permissionxyz")] for example)
- PermissionHandlerSql is a sample implementation of IPermissionHandlerData using SQL Server as the store. It can be configured via PermissionHandlerSqlOptions:
  - ConnectionString
  - PermissionRolesStoredProcedure - stored procedure that returns the mapping
  - TenantField - name of the tenant field returned by the stored procedure. "Tenant" is the default if not specified.
  - PermissionField - name of the permission field returned by the stored procedure. "Permission" is the default if not specified.
  - RoleField - name of the role field returned by the stored procedure. "Role" is the default if not specified.
  - IntervalSeconds - Refresh interval in seconds
- The DbUsers project contains a DB that can be used by PermissionHandlerSql
