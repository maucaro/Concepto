using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;


namespace Vida.Prueba.WebApp
{
  public class PermissionsPolicyProvider : DefaultAuthorizationPolicyProvider
  {
    private readonly AuthorizationOptions _options;

    public PermissionsPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
      _options = options.Value;
    }

    public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
      // Check existing policies first
      var policy = await base.GetPolicyAsync(policyName);

      if (policy == null)
      {
        policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new HasPermission(policyName))
            .Build();

        // Add policy to the AuthorizationOptions, so we don't have to re-create it each time
        _options.AddPolicy(policyName, policy);
      }
      return policy;
    }
  }
}
