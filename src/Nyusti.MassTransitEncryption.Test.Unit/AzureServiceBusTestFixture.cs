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

    /// <summary>
    /// Azure service bus test fixture
    /// </summary>
    /// <seealso cref="MassTransit.TestFramework.BusTestFixture"/>
    [TestFixture]
    public abstract class AzureServiceBusTestFixture : BusTestFixture
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = Logger.Get<AzureServiceBusTestFixture>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusTestFixture"/> class.
        /// </summary>
        /// <param name="inputQueueName">Name of the input queue.</param>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="settings">The settings.</param>
        public AzureServiceBusTestFixture(string inputQueueName = null, Uri serviceUri = null, ServiceBusTokenProviderSettings settings = null)
            : this(new AzureServiceBusTestHarness(
                serviceUri ?? ServiceBusEnvironment.CreateServiceUri("sb", "nyusti", string.Empty),
                settings?.KeyName ?? ((ServiceBusTokenProviderSettings)new BasicAzureServiceBusAccountSettings()).KeyName,
                settings?.SharedAccessKey ?? ((ServiceBusTokenProviderSettings)new BasicAzureServiceBusAccountSettings()).SharedAccessKey,
                inputQueueName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusTestFixture"/> class.
        /// </summary>
        /// <param name="harness">The harness.</param>
        protected AzureServiceBusTestFixture(AzureServiceBusTestHarness harness)
            : base(harness)
        {
            this.AzureServiceBusTestHarness = harness;
            this.AzureServiceBusTestHarness.OnConnectObservers += this.ConnectObservers;
            this.AzureServiceBusTestHarness.OnConfigureServiceBusBus += this.ConfigureServiceBusBus;
            this.AzureServiceBusTestHarness.OnConfigureServiceBusBusHost += this.ConfigureServiceBusBusHost;
            this.AzureServiceBusTestHarness.OnConfigureServiceBusReceiveEndpoint += this.ConfigureServiceBusReceiveEndpoint;
        }

        /// <summary>
        /// Gets the name of the input queue.
        /// </summary>
        /// <value>The name of the input queue.</value>
        protected string InputQueueName => this.AzureServiceBusTestHarness.InputQueueName;

        /// <summary>
        /// Gets the input queue send endpoint.
        /// </summary>
        /// <value>The input queue send endpoint.</value>
        protected ISendEndpoint InputQueueSendEndpoint => this.AzureServiceBusTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// Gets the bus send endpoint.
        /// </summary>
        /// <value>The bus send endpoint.</value>
        protected ISendEndpoint BusSendEndpoint => this.AzureServiceBusTestHarness.BusSendEndpoint;

        /// <summary>
        /// Gets the sent.
        /// </summary>
        /// <value>The sent.</value>
        protected ISentMessageList Sent => this.AzureServiceBusTestHarness.Sent;

        /// <summary>
        /// Gets the bus address.
        /// </summary>
        /// <value>The bus address.</value>
        protected Uri BusAddress => this.AzureServiceBusTestHarness.BusAddress;

        /// <summary>
        /// Gets the input queue address.
        /// </summary>
        /// <value>The input queue address.</value>
        protected Uri InputQueueAddress => this.AzureServiceBusTestHarness.InputQueueAddress;

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        protected IServiceBusHost Host => this.AzureServiceBusTestHarness.Host;

        /// <summary>
        /// Gets the azure service bus test harness.
        /// </summary>
        /// <value>The azure service bus test harness.</value>
        protected AzureServiceBusTestHarness AzureServiceBusTestHarness { get; }

        /// <summary>
        /// Setups the azure service bus test fixture.
        /// </summary>
        /// <returns>Task reference</returns>
        [OneTimeSetUp]
        public Task SetupAzureServiceBusTestFixture()
        {
            return this.AzureServiceBusTestHarness.Start();
        }

        /// <summary>
        /// Tears down in memory test fixture.
        /// </summary>
        /// <returns>Task reference</returns>
        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return this.AzureServiceBusTestHarness.Stop();
        }

        /// <summary>
        /// Configures the service bus bus.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        /// <summary>
        /// Configures the service bus bus host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="host">The host.</param>
        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
        }

        /// <summary>
        /// Configures the service bus receive endpoint.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }
    }
}