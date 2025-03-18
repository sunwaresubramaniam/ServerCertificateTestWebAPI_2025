namespace ServerCertificateTestWebAPI_2025.Interface
{
    public interface ITFLogger
    {
        void LogInfo(string message);
        void LogError(string message, Exception ex);
        void LogDebug(string message);
        void LogWarning(string message, Exception ex);
        void LogCritical(string message, Exception ex);
        void LogTrace(string message, Exception ex);
    }
}
