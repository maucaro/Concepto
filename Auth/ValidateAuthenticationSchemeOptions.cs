using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace Vida.Prueba.Auth
{
  public class ValidateAuthenticationSchemeOptions : AuthenticationSchemeOptions
  {
    public SignedTokenVerificationOptions TokenVerificationOptions { get; set; }
    public List<string> ValidTenants { get; set; }
  }
}
