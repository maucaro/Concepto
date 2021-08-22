using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Vida.Prueba.WebApp.Pages
{
  [Authorize(Roles = "Admin")]
  [Authorize(Policy = "AdministerUsers")]
  public class PrivacyModel : PageModel
  {
    private readonly ILogger<PrivacyModel> _logger;

    public PrivacyModel(ILogger<PrivacyModel> logger)
    {
      _logger = logger;
    }

    public void OnGet()
    {
    }
  }
}
