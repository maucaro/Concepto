using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Vida.Prueba.WebApp
{
  public class DbClaimsTransformation : IClaimsTransformation
  {
    private const string _emailClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
    private readonly IUserGroups _userGroups;
    public DbClaimsTransformation(IUserGroups userGroups)
    {
      _userGroups = userGroups;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
      string email = principal.Claims.First(claim => claim.Type == _emailClaimType).Value;
      var groups = _userGroups.GetGroups(email);
      foreach (string group in groups)
      {
        if (!principal.HasClaim(ClaimTypes.Role, group))
        {
          ClaimsIdentity claimsIdentity = new();
          claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, group));
          principal.AddIdentity(claimsIdentity);
        }
      }
      return Task.FromResult(principal);
    }
  }
}
