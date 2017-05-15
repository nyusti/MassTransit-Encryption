namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Core;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class AzureKeyVaultEncryptionTest : AzureServiceBusTestFixture
    {
        private Task<ConsumeContext<PingMessage>> _handler;

        [Test]
        public async Task Should_succeed()
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
            const string testKeyId = "TestKey";

            var keyResolver = Substitute.For<IKeyResolver>();
            keyResolver.ResolveKeyAsync(testKeyId, TestCancellationToken).Returns(new Task<IKey>(() => new RsaKey(testKeyId)));

            configurator.UseAzureKeyVaultEncryption(kv =>
            {
                kv.EncryptionKey = () => new RsaKey(testKeyId);
                kv.KeyResolver = () => keyResolver;
            });

            base.ConfigureServiceBusBusHost(configurator, host);
        }
    }
}