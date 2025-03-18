using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MTLS_Cert_Lib;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace ServerCertificateTestWebAPI_2025
{
    public class CertificateAuthenicateMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CertificateValidationMiddleware> _logger;
        private readonly HttpContextService _contextService;

        public CertificateAuthenicateMiddleware(HttpContextService contextService, RequestDelegate next, ILogger<CertificateValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _contextService = contextService;
        }

        public async Task Invoke(HttpContext context)
        {
            var clientcert = context.Connection.ClientCertificate;
            //X509Certificate2 serverCert = MTLSServerCertification.GetServerCertificateFromLocalPersonalStore("CN=sunwareServer");
            //bool isValidServerCertificate = MTLSServerCertification.ValidateServerCertificate(serverCert);
            MTLSServerCertification.Server_Certificate_SubjectName = "sunwareServer";
            X509Certificate2 serverCert = MTLSServerCertification.GetServerCertificateFromLocalPersonalStore(MTLSServerCertification.Server_Certificate_SubjectName,null);
            bool isValidServerCertificate = MTLSServerCertification.AuthenticateServer(null,null, "Sunware-mTLSCertificate");
            if (isValidServerCertificate == false)
            {
                _logger.LogWarning("No server certificate provided/ Invalid Server Certificate/ Server Certificate is expired");
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Server certificate required/Expired.");
                return;
            }
            context.Connection.ClientCertificate = serverCert;

        //    // Create ClaimsIdentity
        //    var claims = new[]
        //    {
        //    new Claim(ClaimTypes.Name, serverCert.Subject),
        //    new Claim(ClaimTypes.Thumbprint, serverCert.Thumbprint)
        //};

        //    var identity = new ClaimsIdentity(claims, serverCert.SerialNumber);
        //    var principal = new ClaimsPrincipal(identity);
        //    var ticket = new AuthenticationTicket(principal, serverCert.SerialNumber);

            

            _logger.LogInformation("Server certificate validated successfully.");
            await _next(context);
        }

    }
}
