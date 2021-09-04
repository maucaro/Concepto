using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;

namespace Vida.Prueba.Authentication
{
  public class ValidateAuthenticationSchemeOptions : AuthenticationSchemeOptions
  {
    public SignedTokenVerificationOptions TokenVerificationOptions { get; set; }
  }
}
