using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Maucaro.Auth.IdentityPlatform;

namespace Maucaro.Auth.IdentityPlatform.Sample
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
      services.Configure<PermissionHandlerSqlOptions>(Configuration.GetSection("DbUsers"));
      services.AddSingleton<IPermissionHandlerData, PermissionHandlerSql>();
      services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
      services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
      services.AddAuthorization();
      services
      .AddAuthentication(options => { options.DefaultScheme = CustomAuthenticationDefaults.AuthenticationScheme; })
      .AddCustomAuth(options =>
      {
        Configuration.GetSection("AuthOptions").Bind(options);
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
