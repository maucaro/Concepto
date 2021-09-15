using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Vida.Prueba.Auth
{
  public class ValidateAuthenticationHandler : AuthenticationHandler<ValidateAuthenticationSchemeOptions>
  {
    public ValidateAuthenticationHandler(
        IOptionsMonitor<ValidateAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
      {
        return AuthenticateResult.Fail("Authorization header missing");
      }
      var rawToken = ExtractRawToken(Request.Headers["Authorization"].ToString());
      if (string.IsNullOrWhiteSpace(rawToken))
      {
        return AuthenticateResult.Fail("Bearer token missing");
      }
      try
      {
        TokenClaims tokenClaims = await JsonWebSignature.VerifySignedTokenAsync<TokenClaims>(rawToken, Options.TokenVerificationOptions);
        if (string.IsNullOrWhiteSpace(tokenClaims.sub) || string.IsNullOrWhiteSpace(tokenClaims.email))
        {
          return AuthenticateResult.Fail("Error validating token: 'sub' and 'email' claims are required");
        }
        var tenant = tokenClaims.firebase?.tenant ?? string.Empty;
        if (!Options.ValidTenants.Contains(tenant))
        {
          return AuthenticateResult.Fail("Error validating token: JWT contains invalid 'tenant' claim.");
        }
        List<Claim> claims = new()
        {
          new Claim(ClaimTypes.NameIdentifier, tokenClaims.sub),
          new Claim(ClaimTypes.Email, tokenClaims.email),
          new Claim(CustomAuthenticationDefaults.TenantClaim, tenant)
        };
        if (tokenClaims.roles != null)
        {
          foreach (string role in tokenClaims.roles)
          {
            claims.Add(new Claim(ClaimTypes.Role, role));
          }
        }
        var claimsIdentity = new ClaimsIdentity(claims, nameof(ValidateAuthenticationHandler));
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
        return AuthenticateResult.Success(ticket);
      }
      catch (InvalidJwtException ex)
      {
        return AuthenticateResult.Fail($"Error validating token: {ex.Message}");
      }
      catch (Exception ex)
      {
        return AuthenticateResult.Fail($"Error during token validation: {ex.Message}");
      }
    }

    private static string ExtractRawToken(string Header)
    {
      if (string.IsNullOrWhiteSpace(Header))
      {
        return string.Empty;
      }
      string[] splitHeader = Header.ToString().Split(' ');
      if (splitHeader.Length != 2)
      {
        return string.Empty;
      }
      var scheme = splitHeader[0];
      var token = splitHeader[1];
      if (string.IsNullOrWhiteSpace(token) || scheme.ToLowerInvariant() != "bearer")
      {
        return string.Empty;
      }
      return token;
    }

    private class TokenClaims : JsonWebSignature.Payload
    {
      public string sub { get; set; }

      public string email { get; set; }

      public List<string> roles { get; set; }

      public FirbaseClaim firebase { get; set; }

    }

    private class FirbaseClaim
    {
      public string tenant { get; set; }
    }
  }
}