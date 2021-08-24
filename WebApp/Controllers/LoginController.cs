using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace Vida.Prueba.WebApp.Controllers
{
  public class LoginController : Controller
  {
    [Authorize]
    [HttpPost("/setcookie")]
    public async Task<ActionResult> SignInWithCookie()
    {
      //await HttpContext.SignInAsync( CookieAuthenticationDefaults.AuthenticationScheme, HttpContext.User);
      string token = await HttpContext.GetTokenAsync("access_token");
      HttpContext.Response.Cookies.Append("firebaseAccessToken", token, new Microsoft.AspNetCore.Http.CookieOptions()
      { HttpOnly = true, IsEssential = true, /*SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax, Secure = true , */Path = "/"}
      );
      return StatusCode(403, @"{ 'location' : 'https://localhost:49155/'}" );
      //return Redirect("https://localhost:49155/");
    }
  }
}
