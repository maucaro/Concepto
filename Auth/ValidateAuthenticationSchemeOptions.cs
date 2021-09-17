using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Vida.Prueba.Auth
{
  public class ValidateAuthenticationSchemeOptions : AuthenticationSchemeOptions
  {
    public string CertificatesUrl { get; set; }
    public string TrustedAudience { get; set; }
    public List<string> ValidTenants { get; set; }
  }
}
