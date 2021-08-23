using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

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
      //services.AddSingleton<IUserGroups, UserGroups>();
      //services.AddTransient<IClaimsTransformation, DbClaimsTransformation>();
      services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
      services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
      services.AddAuthorization();
      services
      .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
      .AddJwtBearer(options =>
      {
        options.Authority = Configuration.GetSection("JwtOptions").GetValue<string>("Authority");
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = Configuration.GetSection("JwtOptions").GetValue<string>("Issuer"),
          ValidateAudience = true,
          ValidAudience = Configuration.GetSection("JwtOptions").GetValue<string>("Audience"),
          ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            context.Token = context.Request.Cookies[Configuration.GetSection("JwtOptions").GetValue<string>("TokenCookie")];
            return Task.CompletedTask;
          }
        };
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseStatusCodePages(context =>
      {
        var request = context.HttpContext.Request;
        var response = context.HttpContext.Response;

        if (response.StatusCode == (int)System.Net.HttpStatusCode.Unauthorized)
        {
          response.Redirect(Configuration.GetValue<string>("LoginPage"));
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
