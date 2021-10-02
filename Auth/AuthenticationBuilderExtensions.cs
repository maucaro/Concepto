using Microsoft.AspNetCore.Authentication;
using System;

namespace Maucaro.Auth.IdentityPlatform
{
  public static class AuthenticationBuilderExtensions
  {
    public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<ValidateAuthenticationSchemeOptions> configureOptions)
    {
      return builder.AddScheme<ValidateAuthenticationSchemeOptions, ValidateAuthenticationHandler>
          (CustomAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

  }
}
