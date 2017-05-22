namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.TestFramework;
    using NUnit.Framework;

    /// <summary>
    /// Azure key vault encryption tests
    /// </summary>
    /// <seealso cref="MassTransit.TestFramework.InMemoryTestFixture"/>
    [TestFixture]
    public class KeyVaultEncryptionOnInMemoryBusTest : InMemoryTestFixture
    {
        /// <summary>
        /// The handler
        /// </summary>
        private Task<ConsumeContext<PingMessage>> handler;

        /* disabled until header handling is fixed
        [Test]
        */

        /// <summary>
        /// Ins the memory bus encrypt message.
        /// </summary>
        /// <returns>Task reference</returns>
        public async Task InMemoryBus_EncryptMessage()
        {
            await this.Bus.Publish(new PingMessage()).ConfigureAwait(false);
            ConsumeContext<PingMessage> received = await this.handler.ConfigureAwait(false);
            Assert.AreEqual(AzureKeyVault.EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        /// <inheritdoc/>
        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            this.handler = base.Handled<PingMessage>(configurator);
        }

        /// <inheritdoc/>
        protected override void PreCreateBus(IInMemoryBusFactoryConfigurator configurator)
        {
            var encrpytionConfiguration = new TestKeyProvider();

            configurator.UseAzureKeyVaultEncryption((kv) =>
            {
                kv.EncryptionKey = encrpytionConfiguration.EncryptionKey;
                kv.KeyResolver = encrpytionConfiguration.KeyResolver;
            });

            base.PreCreateBus(configurator);
        }
    }
}