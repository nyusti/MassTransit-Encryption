namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using Microsoft.ServiceBus;

    /// <summary>
    /// Service bus token provider interface
    /// </summary>
    public interface IAzureServiceBusTokenProvider
    {
        /// <summary>
        /// Gets the token provider.
        /// </summary>
        /// <returns>The token provider</returns>
        TokenProvider GetTokenProvider();
    }
}