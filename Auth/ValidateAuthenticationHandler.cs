using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
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
      string certificatesUrl = Options.CertificatesUrl ?? CustomAuthenticationDefaults.CertificatesUrl;
      // TrustedAudiences must be provided
      string trustedAudience = Options.TrustedAudience;
      if (trustedAudience == null)
      {
        return AuthenticateResult.Fail("Error validating token: AuthOptions:TrustedAudience needs to be set");
      }
      // If ValidTenants == null, no tenant verification will be done
      List<string> validTenants = Options.ValidTenants;

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
        SignedTokenVerificationOptions tokenOptions = new ()
        {
          // Use clock tolerance to account for possible clock differences
          // between the issuer and the verifier.
          IssuedAtClockTolerance = TimeSpan.FromMinutes(1),
          ExpiryClockTolerance = TimeSpan.FromMinutes(1),
          CertificatesUrl = certificatesUrl,
          TrustedAudiences = { trustedAudience }
        };
        TokenClaims tokenClaims = await JsonWebSignature.VerifySignedTokenAsync<TokenClaims>(rawToken, tokenOptions);
        if (string.IsNullOrWhiteSpace(tokenClaims.sub) || string.IsNullOrWhiteSpace(tokenClaims.email))
        {
          return AuthenticateResult.Fail("Error validating token: 'sub' and 'email' claims are required");
        }
        var tenant = tokenClaims.firebase?.tenant;

        // Only check for valid tenants if option has been set
        if (validTenants != null && !validTenants.Contains(tenant ?? string.Empty))
        {
          return AuthenticateResult.Fail("Error validating token: JWT contains invalid 'tenant' claim.");
        }
        List<Claim> claims = new()
        {
          new Claim(ClaimTypes.NameIdentifier, tokenClaims.sub),
          new Claim(ClaimTypes.Email, tokenClaims.email)
        };
        if (tokenClaims.name != null)
        {
          claims.Add(new Claim(ClaimTypes.Name, tokenClaims.name));
        }
        if (tenant != null)
        {
          claims.Add(new Claim(CustomAuthenticationDefaults.TenantClaim, tenant));
        }
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

      public string name { get; set; }

      public List<string> roles { get; set; }

      public FirbaseClaim firebase { get; set; }
    }

    private class FirbaseClaim
    {
      public string tenant { get; set; }
    }
  }
}