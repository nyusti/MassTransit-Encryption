namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using MassTransit.Logging;
    using MassTransit.Testing;
    using Microsoft.ServiceBus;

    /// <summary>
    /// Azure service bus test harness
    /// </summary>
    /// <seealso cref="MassTransit.Testing.BusTestHarness"/>
    public class AzureServiceBusTestHarness : BusTestHarness
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = Logger.Get<AzureServiceBusTestHarness>();

        /// <summary>
        /// The service URI
        /// </summary>
        private readonly Uri serviceUri;

        /// <summary>
        /// The input queue address
        /// </summary>
        private Uri inputQueueAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureServiceBusTestHarness"/> class.
        /// </summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="sharedAccessKeyName">Name of the shared access key.</param>
        /// <param name="sharedAccessKeyValue">The shared access key value.</param>
        /// <param name="inputQueueName">Name of the input queue.</param>
        /// <exception cref="System.ArgumentNullException">serviceUri</exception>
        public AzureServiceBusTestHarness(Uri serviceUri, string sharedAccessKeyName, string sharedAccessKeyValue, string inputQueueName = null)
        {
            this.serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            this.SharedAccessKeyName = sharedAccessKeyName;
            this.SharedAccessKeyValue = sharedAccessKeyValue;

            this.TokenTimeToLive = TimeSpan.FromDays(1);
            this.TokenScope = TokenScope.Namespace;

            this.InputQueueName = inputQueueName ?? "input_queue";

            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;
        }

        /// <summary>
        /// Gets or sets gets the on configure service bus bus
        /// </summary>
        public Action<IServiceBusBusFactoryConfigurator> OnConfigureServiceBusBus { get; set; }

        /// <summary>
        /// Gets or sets gets occurs when [on configure service bus bus host].
        /// </summary>
        public Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> OnConfigureServiceBusBusHost { get; set; }

        /// <summary>
        /// Gets or sets gets occurs when [on configure service bus receive endpoint].
        /// </summary>
        public Action<IServiceBusReceiveEndpointConfigurator> OnConfigureServiceBusReceiveEndpoint { get; set; }

        /// <summary>
        /// Gets the name of the shared access key.
        /// </summary>
        /// <value>The name of the shared access key.</value>
        public string SharedAccessKeyName { get; }

        /// <summary>
        /// Gets the shared access key value.
        /// </summary>
        /// <value>The shared access key value.</value>
        public string SharedAccessKeyValue { get; }

        /// <summary>
        /// Gets or sets the token time to live.
        /// </summary>
        /// <value>The token time to live.</value>
        public TimeSpan TokenTimeToLive { get; set; }

        /// <summary>
        /// Gets or sets the token scope.
        /// </summary>
        /// <value>The token scope.</value>
        public TokenScope TokenScope { get; set; }

        /// <summary>
        /// Gets the name of the input queue.
        /// </summary>
        /// <value>The name of the input queue.</value>
        public string InputQueueName { get; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IServiceBusHost Host { get; private set; }

        /// <inheritdoc/>
        public override Uri InputQueueAddress => this.inputQueueAddress;

        /// <summary>
        /// Configures the service bus bus.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            this.OnConfigureServiceBusBus?.Invoke(configurator);
        }

        /// <summary>
        /// Configures the service bus bus host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="host">The host.</param>
        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            this.OnConfigureServiceBusBusHost?.Invoke(configurator, host);
        }

        /// <summary>
        /// Configures the service bus receive endpoint.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            this.OnConfigureServiceBusReceiveEndpoint?.Invoke(configurator);
        }

        /// <inheritdoc/>
        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                this.Host = x.Host(this.serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = this.SharedAccessKeyName;
                        s.SharedAccessKey = this.SharedAccessKeyValue;
                        s.TokenTimeToLive = this.TokenTimeToLive;
                        s.TokenScope = this.TokenScope;
                    });
                });

                this.ConfigureBus(x);
                this.ConfigureServiceBusBus(x);
                x.UseServiceBusMessageScheduler();
                this.ConfigureServiceBusBusHost(x, this.Host);

                x.ReceiveEndpoint(this.Host, this.InputQueueName, e =>
                {
                    this.ConfigureReceiveEndpoint(e);
                    this.ConfigureServiceBusReceiveEndpoint(e);
                    this.inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}