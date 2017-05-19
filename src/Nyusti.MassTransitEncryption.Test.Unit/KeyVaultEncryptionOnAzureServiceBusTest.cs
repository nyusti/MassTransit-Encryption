namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using NUnit.Framework;

    /// <summary>
    /// Azure key vault encryption tests
    /// </summary>
    /// <seealso cref="Nyusti.MassTransitEncryption.Test.Unit.AzureServiceBusTestFixture"/>
    [TestFixture]
    public class KeyVaultEncryptionOnAzureServiceBusTest : AzureServiceBusTestFixture
    {
        /// <summary>
        /// The handler
        /// </summary>
        private Task<ConsumeContext<PingMessage>> handler;

        /* disabled until header handling is fixed
        [Test]
        */

        /// <summary>
        /// Azures the service bus encrypt message.
        /// </summary>
        /// <returns>Task reference</returns>
        public async Task AzureServiceBus_EncryptMessage()
        {
            await Bus.Publish(new PingMessage()).ConfigureAwait(false);
            ConsumeContext<PingMessage> received = await this.handler.ConfigureAwait(false);
            Assert.AreEqual(AzureKeyVault.EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        /// <summary>
        /// Configures the service bus receive endpoint.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            this.handler = base.Handled<PingMessage>(configurator);
        }

        /// <summary>
        /// Configures the service bus bus host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="host">The host.</param>
        protected override void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            var encrpytionConfiguration = new TestKeyProvider();

            configurator.UseAzureKeyVaultEncryption((kv) =>
            {
                kv.EncryptionKey = encrpytionConfiguration.EncryptionKey;
                kv.KeyResolver = encrpytionConfiguration.KeyResolver;
            });

            base.ConfigureServiceBusBusHost(configurator, host);
        }
    }
}