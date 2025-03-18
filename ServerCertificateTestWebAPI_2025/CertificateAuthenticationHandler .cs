using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using MTLS_Cert_Lib;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Encodings.Web;

namespace ServerCertificateTestWebAPI_2025
{
    public class CertificateAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        
        public CertificateAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var clientCertificate = Context.Connection.ClientCertificate;

            if (clientCertificate == null)
            {
                return AuthenticateResult.Fail("Client certificate is required.");
            }

            if (!ValidateCertificate(clientCertificate))
            {
                return AuthenticateResult.Fail("Invalid client certificate.");
            }

            // Create ClaimsIdentity
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, clientCertificate.Subject),
            new Claim(ClaimTypes.Thumbprint, clientCertificate.Thumbprint)
        };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private bool ValidateCertificate(X509Certificate2 certificate)
        {
            MTLSServerCertification.Server_Certificate_SubjectName = "sunwareServer";
            MTLSServerCertification.Server_Certificate_Issuer = "Sunware-mTLSCertificate";
            bool isValidServerCertificate = MTLSServerCertification.ValidateServerCertificate(certificate);
            return isValidServerCertificate;
        }
    }
}