namespace Vida.Prueba.Auth
{
  public class CustomAuthenticationDefaults
  {
    public const string AuthenticationScheme = "CustomAuthentication";
    public const string CertificatesUrl = "https://www.googleapis.com/service_accounts/v1/jwk/securetoken@system.gserviceaccount.com";
    // TenantClain will be used as the value for Claim.type for the tenant claim to be added to ClaimsIdentity
    public const string TenantClaim = "tenant";
  }
}
