namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.TestFramework;
    using NUnit.Framework;

    [TestFixture]
    public class AzureKeyVaultEncryptionTest : InMemoryTestFixture
    {
        private Task<ConsumeContext<PingMessage>> _handler;

        [Test]
        public async Task InMemoryBus_EncryptMessage()
        {
            await Bus.Publish(new PingMessage());

            ConsumeContext<PingMessage> received = await _handler;

            Assert.AreEqual(AzureKeyVault.EncryptedMessageSerializer.EncryptedContentType, received.ReceiveContext.ContentType);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _handler = Handled<PingMessage>(configurator);
        }

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