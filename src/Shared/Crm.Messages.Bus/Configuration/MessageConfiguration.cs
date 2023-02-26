namespace Crm.Messages.Bus.Configuration
{
    public record MessageConfiguration(
        string Host = "localhost",
        string VirtualHost = "/",
        string Username = "guest",
        string Password = "guest",
        int RetryCount = 3,
        int RetryIntervalSeconds = 5,
        int TimeoutSeconds = 30);
}
