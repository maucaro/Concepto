using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vida.Prueba.WebApp.Controllers
{
  public class RefreshTokenController : Controller
  {
    private static readonly HttpClient _client = new();
    private readonly string _tokenCookie;
    private readonly int _sessionSlidingTimeoutMinutes;
    private readonly string _apiKey;
    private readonly string _firebaseRefreshTokenUrl;
    public RefreshTokenController(IConfiguration configuration)
    {
      _tokenCookie = configuration.GetSection("JwtOptions").GetValue<string>("TokenCookie");
      _sessionSlidingTimeoutMinutes = configuration.GetValue<int>("SessionSlidingTimeoutMinutes", 5);
    }

    [Authorize]
    [HttpPost("/refreshtoken")]
    public async Task<ActionResult> RefreshToken()
    {
      var context = HttpContext;
      string idToken;
      using (StreamReader reader = new StreamReader(context.Request.Body))
      {
        idToken = await reader.ReadToEndAsync();
      }
      context.Response.Cookies.Append(_tokenCookie, idToken, new CookieOptions()
      { IsEssential = true, HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(_sessionSlidingTimeoutMinutes) });
      return Ok();
    }
  }
}