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
            var message = new PingMessage();
            await this.Bus.Publish(message).ConfigureAwait(false);

            var received = await this.handler.ConfigureAwait(false);

            Assert.AreEqual(AzureKeyVault.EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
            Assert.AreEqual(message, received.Message);
        }

        /// <inheritdoc/>
        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            this.handler = base.Handled<PingMessage>(configurator);
        }

        /// <inheritdoc/>
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