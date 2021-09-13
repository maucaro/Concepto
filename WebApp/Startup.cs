using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vida.Prueba.Auth;

namespace Vida.Prueba.WebApp
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddRazorPages();
      services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
      services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
      services.AddAuthorization();
      services
      .AddAuthentication(options => { options.DefaultScheme = CustomAuthenticationDefaults.AuthenticationScheme; })
      .AddCustomAuth(options =>
      {
        SignedTokenVerificationOptions tokenOptions = new();
        Configuration.GetSection("AuthOptions:TokenVerificationOptions").Bind(tokenOptions);
        options.TokenVerificationOptions = tokenOptions;
        options.ValidTenants = Configuration.GetSection("AuthOptions:ValidTenants").Get<List<string>>();
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      string loginPage = Configuration.GetValue<string>("LoginPage");

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      app.UseStaticFiles();
      app.UseStatusCodePages(context =>
      {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
        {
          response.Redirect(loginPage);
        }
        return Task.CompletedTask;
      });
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapRazorPages();
      });
    }
  }
}
