using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Vida.Prueba.Auth.UnitTests
{
  [TestClass]
  public class UnitTestAuthHandler
  {
    private const string _testUser = "test@maucaro.com";
    private const string _environmentName = "Development";
    private readonly Mock<IOptionsMonitor<ValidateAuthenticationSchemeOptions>> _options;
    private readonly Mock<ILoggerFactory> _loggerFactory;
    private readonly Mock<UrlEncoder> _encoder;
    private readonly Mock<ISystemClock> _clock;
    private readonly ValidateAuthenticationHandler _handler;

    public UnitTestAuthHandler()
    {
      var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .AddJsonFile($"appsettings.{_environmentName}.json", true, true)
          .Build();
      SignedTokenVerificationOptions tokenOptions = new();
      configuration.GetSection("AuthOptions:TokenVerificationOptions").Bind(tokenOptions);
      ValidateAuthenticationSchemeOptions options = new();
      options.TokenVerificationOptions = tokenOptions;
      options.ValidTenants = configuration.GetSection("AuthOptions:ValidTenants").Get<List<string>>();

      _options = new Mock<IOptionsMonitor<ValidateAuthenticationSchemeOptions>>();

      // This Setup is required for .NET Core 3.1 onwards.
      _options
          .Setup(x => x.Get(It.IsAny<string>()))
          .Returns(options);

      var logger = new Mock<ILogger<ValidateAuthenticationHandler>>();
      _loggerFactory = new Mock<ILoggerFactory>();
      _loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

      _encoder = new Mock<UrlEncoder>();
      _clock = new Mock<ISystemClock>();

      _handler = new ValidateAuthenticationHandler(_options.Object, _loggerFactory.Object, _encoder.Object, _clock.Object);
    }


    private static string ReadTokenFromFile(string file)
    {
      string token = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/gen-jwt/" + file);
      // Removes CR+LF
      return token.Remove(token.Length - 2);
    }
    private async Task TestMiddleware_Fail(string header, string expectedMessage = null)
    {
      var context = new DefaultHttpContext();
      if (!string.IsNullOrWhiteSpace(header))
      {
        context.Request.Headers.Add("authorization", header);
      }

      await _handler.InitializeAsync(new AuthenticationScheme(CustomAuthenticationDefaults.AuthenticationScheme, null, typeof(ValidateAuthenticationHandler)), context);
      var result = await _handler.AuthenticateAsync();
      Assert.IsFalse(result.Succeeded);
      Assert.AreEqual(result.Failure.Message, expectedMessage);
    }

    [TestMethod]
    public async Task Test_NoAuthorizationHeader()
    {
      await TestMiddleware_Fail(string.Empty, "Authorization header missing");
    }

    [TestMethod]
    public async Task Test_SchemeNotBearer()
    {
      await TestMiddleware_Fail("basic xyz", "Bearer token missing");
    }

    [TestMethod]
    public async Task Test_TokenWithNoPayload()
    {
      await TestMiddleware_Fail("bearer xyz", "Error validating token: JWT must consist of Header, Payload, and Signature");
    }

    [TestMethod]
    public async Task Test_TokenPayloadNotDecodable()
    {
      await TestMiddleware_Fail("bearer x.y.z", "Error during token validation: The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.");
    }

    [TestMethod]
    public async Task Test_TokenExpired()
    {
      string token = ReadTokenFromFile("token_expired");
      await TestMiddleware_Fail("bearer " + token, "Error validating token: JWT has expired.");
    }

    [TestMethod]
    public async Task Test_TokenNoEmail()
    {
      string token = ReadTokenFromFile("token_no_email");
      await TestMiddleware_Fail("bearer " + token, "Error validating token: 'sub' and 'email' claims are required");
    }

    [TestMethod]
    public async Task Test_TokenWrongAudience()
    {
      string token = ReadTokenFromFile("token_wrong_aud");
      await TestMiddleware_Fail("bearer " + token, "Error validating token: JWT contains untrusted 'aud' claim.");
    }

    [TestMethod]
    public async Task Test_TokenWrongTenant()
    {
      string token = ReadTokenFromFile("token_wrong_tenant");
      await TestMiddleware_Fail("bearer " + token, "Error validating token: JWT contains invalid 'tenant' claim.");
    }

    [TestMethod]
    public async Task Test_TokenWrongKey()
    {
      string token = ReadTokenFromFile("token_wrong_key");
      await TestMiddleware_Fail("bearer " + token, "Error validating token: JWT invalid, unable to verify signature.");
    }

    [TestMethod]
    public async Task Test_Authenticated()
    {
      string token = ReadTokenFromFile("token");
      var context = new DefaultHttpContext();
      context.Request.Headers.Add("authorization", "bearer " + token);

      await _handler.InitializeAsync(new AuthenticationScheme(CustomAuthenticationDefaults.AuthenticationScheme, null, typeof(ValidateAuthenticationHandler)), context);
      var result = await _handler.AuthenticateAsync();

      Assert.IsTrue(result.Succeeded);
      Assert.AreEqual(result.Principal.Claims.First(v => v.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value, _testUser);
    }

    [TestMethod]
    public async Task Test_NoTenant()
    {
      string token = ReadTokenFromFile("token_no_tenant");
      var context = new DefaultHttpContext();
      context.Request.Headers.Add("authorization", "bearer " + token);

      await _handler.InitializeAsync(new AuthenticationScheme(CustomAuthenticationDefaults.AuthenticationScheme, null, typeof(ValidateAuthenticationHandler)), context);
      var result = await _handler.AuthenticateAsync();

      Assert.IsTrue(result.Succeeded);
      Assert.AreEqual(result.Principal.Claims.First(v => v.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value, _testUser);
    }
  }
}
