using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;

namespace Vida.Prueba.Auth
{
  public class ValidateAuthenticationSchemeOptions : AuthenticationSchemeOptions
  {
    public SignedTokenVerificationOptions TokenVerificationOptions { get; set; }
  }
}
