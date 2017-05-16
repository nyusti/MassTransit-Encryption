namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using NUnit.Framework;

    [TestFixture]
    public class KeyVaultEncryptionOnAzureServiceBusTest : AzureServiceBusTestFixture
    {
        private Task<ConsumeContext<PingMessage>> _handler;

        [Test]
        public async Task AzureServiceBus_EncryptMessage()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(AzureKeyVault.EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        protected override void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

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