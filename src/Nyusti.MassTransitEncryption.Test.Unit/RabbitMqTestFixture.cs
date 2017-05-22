namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using MassTransit.TestFramework;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using NUnit.Framework;
    using RabbitMQ.Client;

    /// <summary>
    /// RabbitMQ test fixture
    /// </summary>
    /// <seealso cref="MassTransit.TestFramework.BusTestFixture"/>
    public class RabbitMqTestFixture : BusTestFixture
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqTestFixture"/> class.
        /// </summary>
        /// <param name="logicalHostAddress">The logical host address.</param>
        /// <param name="inputQueueName">Name of the input queue.</param>
        public RabbitMqTestFixture(Uri logicalHostAddress = null, string inputQueueName = null)
            : this(new RabbitMqTestHarness(inputQueueName), logicalHostAddress)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqTestFixture"/> class.
        /// </summary>
        /// <param name="harness">The harness.</param>
        /// <param name="logicalHostAddress">The logical host address.</param>
        public RabbitMqTestFixture(RabbitMqTestHarness harness, Uri logicalHostAddress = null)
            : base(harness)
        {
            this.RabbitMqTestHarness = harness;

            if (logicalHostAddress != null)
            {
                this.RabbitMqTestHarness.NodeHostName = this.RabbitMqTestHarness.HostAddress.Host;
                this.RabbitMqTestHarness.HostAddress = logicalHostAddress;
            }

            this.RabbitMqTestHarness.OnConfigureRabbitMqHost += this.ConfigureRabbitMqHost;
            this.RabbitMqTestHarness.OnConfigureRabbitMqBus += this.ConfigureRabbitMqBus;
            this.RabbitMqTestHarness.OnConfigureRabbitMqBusHost += this.ConfigureRabbitMqBusHost;
            this.RabbitMqTestHarness.OnConfigureRabbitMqReceiveEndoint += this.ConfigureRabbitMqReceiveEndoint;
            this.RabbitMqTestHarness.OnCleanupVirtualHost += this.OnCleanupVirtualHost;
        }

        /// <summary>
        /// Gets the input queue send endpoint.
        /// </summary>
        /// <value>The input queue send endpoint.</value>
        protected ISendEndpoint InputQueueSendEndpoint => this.RabbitMqTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// Gets the input queue address.
        /// </summary>
        /// <value>The input queue address.</value>
        protected Uri InputQueueAddress => this.RabbitMqTestHarness.InputQueueAddress;

        /// <summary>
        /// Gets the host address.
        /// </summary>
        /// <value>The host address.</value>
        protected Uri HostAddress => this.RabbitMqTestHarness.HostAddress;

        /// <summary>
        /// Gets the bus send endpoint.
        /// </summary>
        /// <value>The bus send endpoint.</value>
        protected ISendEndpoint BusSendEndpoint => this.RabbitMqTestHarness.BusSendEndpoint;

        /// <summary>
        /// Gets the sent.
        /// </summary>
        /// <value>The sent.</value>
        protected ISentMessageList Sent => this.RabbitMqTestHarness.Sent;

        /// <summary>
        /// Gets the bus address.
        /// </summary>
        /// <value>The bus address.</value>
        protected Uri BusAddress => this.RabbitMqTestHarness.BusAddress;

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        protected IRabbitMqHost Host => this.RabbitMqTestHarness.Host;

        /// <summary>
        /// Gets the name formatter.
        /// </summary>
        /// <value>The name formatter.</value>
        protected IMessageNameFormatter NameFormatter => this.RabbitMqTestHarness.NameFormatter;

        /// <summary>
        /// Gets the rabbit mq test harness.
        /// </summary>
        /// <value>The rabbit mq test harness.</value>
        protected RabbitMqTestHarness RabbitMqTestHarness { get; }

        /// <summary>
        /// Setups the in memory test fixture.
        /// </summary>
        /// <returns>Task reference.</returns>
        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return this.RabbitMqTestHarness.Start();
        }

        /// <summary>
        /// Tears down in memory test fixture.
        /// </summary>
        /// <returns>Task reference</returns>
        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return this.RabbitMqTestHarness.Stop();
        }

        /// <summary>
        /// Configures the rabbit mq host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
        }

        /// <summary>
        /// Configures the rabbit mq bus.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
        }

        /// <summary>
        /// Configures the rabbit mq bus host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="host">The host.</param>
        protected virtual void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
        }

        /// <summary>
        /// Configures the rabbit mq receive endoint.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqReceiveEndoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        /// <summary>
        /// Called when [cleanup virtual host].
        /// </summary>
        /// <param name="model">The model.</param>
        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }
    }
}