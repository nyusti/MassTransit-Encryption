namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using NUnit.Framework;
    using Nyusti.MassTransitEncryption.AzureKeyVault;

    /// <summary>
    /// Key vault encryption test on RabbitMQ
    /// </summary>
    /// <seealso cref="Nyusti.MassTransitEncryption.Test.Unit.RabbitMqTestFixture"/>
    [TestFixture]
    public class KeyVaultEncryptionOnRabbitMqBusTest : RabbitMqTestFixture
    {
        /// <summary>
        /// The handler
        /// </summary>
        private Task<ConsumeContext<PingMessage>> handler;

        /* disabled until header handling is fixed
         * [Test]
        */

        /// <summary>
        /// RabbitMQ encrypt message test.
        /// </summary>
        /// <returns>Task reference</returns>
        public async Task RabbitMqBus_EncryptMessage()
        {
            var message = new PingMessage();
            await this.Bus.Publish(message).ConfigureAwait(false);

            var received = await this.handler.ConfigureAwait(false);

            Assert.AreEqual(EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
            Assert.AreEqual(message, received.Message);
        }

        /// <inheritdoc/>
        protected override void ConfigureRabbitMqReceiveEndoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            this.handler = this.Handled<PingMessage>(configurator);
        }

        /// <inheritdoc/>
        protected override void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            var encrpytionConfiguration = new TestKeyProvider();

            configurator.UseAzureKeyVaultEncryption((kv) =>
            {
                kv.EncryptionKey = encrpytionConfiguration.EncryptionKey;
                kv.KeyResolver = encrpytionConfiguration.KeyResolver;
            });

            base.ConfigureRabbitMqBus(configurator);
        }
    }
}