using System;
using Microsoft.AspNetCore.Authentication;

namespace Vida.Prueba.Auth
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
