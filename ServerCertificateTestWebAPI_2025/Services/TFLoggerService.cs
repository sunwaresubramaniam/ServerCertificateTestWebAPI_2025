using MTLS_Cert_Lib;

namespace ServerCertificateTestWebAPI_2025.Services
{
    public class TFLoggerService
    {
        private readonly ITFLogger _logger;

        public TFLoggerService(ITFLogger logger)  // ✅ Works correctly
        {
            _logger = logger;
        }

        public void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }
        public void LogError(string message)
        {
            _logger.LogError(message);
        }
        public void LogWarning(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
