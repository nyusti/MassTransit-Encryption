namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using Microsoft.ServiceBus;

    public interface IAzureServiceBusTokenProvider
    {
        TokenProvider GetTokenProvider();
    }
}