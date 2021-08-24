using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
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
      string authority = Configuration.GetSection("JwtOptions").GetValue<string>("Authority");
      string issuer = Configuration.GetSection("JwtOptions").GetValue<string>("Issuer");
      string audience = Configuration.GetSection("JwtOptions").GetValue<string>("Audience");
      string tokenCookie = Configuration.GetSection("JwtOptions").GetValue<string>("TokenCookie");
      services.AddRazorPages();
      services.AddControllersWithViews();
      //services.AddSingleton<IUserGroups, UserGroups>();
      //services.AddTransient<IClaimsTransformation, DbClaimsTransformation>();
      services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
      services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
      services.AddAuthorization();
      services
      .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
      .AddJwtBearer(options =>
      {
        options.Authority = authority;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = issuer,
          ValidateAudience = true,
          ValidAudience = audience,
          ValidateLifetime = true
        };
        options.Events = new JwtBearerEvents
        {
          OnMessageReceived = context =>
          {
            if (!String.IsNullOrWhiteSpace(context.Request.Cookies[tokenCookie]))
            {
              context.Token = context.Request.Cookies[tokenCookie];
              context.Response.Cookies.Append(tokenCookie, context.Token, new CookieOptions() 
              { IsEssential = true, HttpOnly = true, Secure = true, Expires = DateTime.Now.AddMinutes(5) });
            }
            return Task.CompletedTask;
          }
        };
      });
      //.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, o =>
      //{
      //  o.ExpireTimeSpan = TimeSpan.FromMinutes(30); // optional
      //});
      //var multiSchemePolicy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
      //  .RequireAuthenticatedUser()
      //  .Build();
      //services.AddAuthorization(o => o.DefaultPolicy = multiSchemePolicy);
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
          response.Redirect(loginPage);
        }
        return Task.CompletedTask;
      });
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        //endpoints.MapControllers();
        endpoints.MapRazorPages();
      });
    }
  }
}
