namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using MassTransit.Logging;
    using MassTransit.TestFramework;
    using MassTransit.Testing;
    using Microsoft.ServiceBus;
    using NUnit.Framework;

    [TestFixture]
    public abstract class AzureServiceBusTestFixture : BusTestFixture
    {
        private static readonly ILog _log = Logger.Get<AzureServiceBusTestFixture>();

        public AzureServiceBusTestFixture(string inputQueueName = null, Uri serviceUri = null, ServiceBusTokenProviderSettings settings = null)
            : this(new AzureServiceBusTestHarness(
                serviceUri ?? ServiceBusEnvironment.CreateServiceUri("sb", "masstransit-build", "MassTransit.AzureServiceBusTransport.Tests"),
                settings?.KeyName ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).KeyName,
                settings?.SharedAccessKey ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).SharedAccessKey,
                inputQueueName))
        {
        }

        protected AzureServiceBusTestFixture(AzureServiceBusTestHarness harness)
            : base(harness)
        {
            AzureServiceBusTestHarness = harness;

            AzureServiceBusTestHarness.OnConnectObservers += ConnectObservers;
            AzureServiceBusTestHarness.OnConfigureServiceBusBus += ConfigureServiceBusBus;
            AzureServiceBusTestHarness.OnConfigureServiceBusBusHost += ConfigureServiceBusBusHost;
            AzureServiceBusTestHarness.OnConfigureServiceBusReceiveEndpoint += ConfigureServiceBusReceiveEndpoint;
        }

        protected string InputQueueName => AzureServiceBusTestHarness.InputQueueName;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AzureServiceBusTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AzureServiceBusTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AzureServiceBusTestHarness.Sent;
        protected Uri BusAddress => AzureServiceBusTestHarness.BusAddress;
        protected Uri InputQueueAddress => AzureServiceBusTestHarness.InputQueueAddress;
        protected IServiceBusHost Host => AzureServiceBusTestHarness.Host;
        protected AzureServiceBusTestHarness AzureServiceBusTestHarness { get; }

        [OneTimeSetUp]
        public Task SetupAzureServiceBusTestFixture()
        {
            return AzureServiceBusTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AzureServiceBusTestHarness.Stop();
        }

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }
    }
}