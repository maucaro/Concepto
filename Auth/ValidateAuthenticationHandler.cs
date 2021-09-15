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
        if (string.IsNullOrWhiteSpace(tokenClaims.Sub) || string.IsNullOrWhiteSpace(tokenClaims.Email))
        {
          return AuthenticateResult.Fail("Error validating token: 'sub' and 'email' claims are required");
        }
        if (!Options.ValidTenants.Contains(tokenClaims.Firebase.Tenant))
        {
          return AuthenticateResult.Fail("Error validating token: JWT contains invalid 'tenant' claim.");
        }
        List<Claim> claims = new()
        {
          new Claim(ClaimTypes.NameIdentifier, tokenClaims.Sub),
          new Claim(ClaimTypes.Email, tokenClaims.Email),
          new Claim(CustomAuthenticationDefaults.TenantClaim, tokenClaims.Firebase.Tenant)
        };

        foreach (string role in tokenClaims.Roles)
        {
          claims.Add(new Claim(ClaimTypes.Role, role));
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
      public TokenClaims(string sub, string email, List<string> roles, FirbaseClaim firebase)
      {
        Sub = sub;
        Email = email;
        Roles = !(roles == null) ? roles : new List<string>();
        Firebase = !(firebase == null) ? firebase : new FirbaseClaim();
      }
      [JsonPropertyName("sub")]
      public string Sub { get; set; }

      [JsonPropertyName("email")]
      public string Email { get; set; }

      [JsonPropertyName("roles")]
      public List<string> Roles { get; set; }

      [JsonPropertyName("firebase")]
      public FirbaseClaim Firebase { get; set; }

    }

    private class FirbaseClaim
    {
      public FirbaseClaim(string tenant = "")
      {
        Tenant = !(String.IsNullOrWhiteSpace(tenant)) ? tenant : String.Empty;
      }
      [JsonPropertyName("tenant")]
      public string Tenant { get; set; }
    }
  }
}