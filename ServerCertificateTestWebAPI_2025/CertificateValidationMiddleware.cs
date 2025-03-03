using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using MTLS_Cert_Lib;

namespace ServerCertificateTestWebAPI_2025
{
    public class CertificateValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CertificateValidationMiddleware> _logger;
        private readonly HttpContextService _contextService;

        public CertificateValidationMiddleware(HttpContextService contextService,RequestDelegate next, ILogger<CertificateValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _contextService = contextService;
        }
        
        public async Task Invoke(HttpContext context)
        {
            //X509Certificate2 serverCert = context.Connection.ClientCertificate;

            //X509Certificate2 serverCert = MTLSServerCertification.GetServerCertificateFromLocalPersonalStore("CN=sunwareServer");
            //bool isValidServerCertificate = MTLSServerCertification.ValidateServerCertificate(serverCert);
            MTLSServerCertification.Server_Certificate_SubjectName = "CN=sunwareServer";
            bool isValidServerCertificate = MTLSServerCertification.ValidateServerCertificate();
            if (isValidServerCertificate  == false)
            {
                _logger.LogWarning("No server certificate provided/ Invalid Server Certificate/ Server Certificate is expired");
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Server certificate required/Expired.");
                return;
            }

           
            _logger.LogInformation("Server certificate validated successfully.");
            await _next(context);
        }
    }
}
