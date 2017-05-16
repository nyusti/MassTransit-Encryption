namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit;
    using MassTransit.AzureServiceBusTransport;
    using MassTransit.Logging;
    using MassTransit.Testing;
    using Microsoft.ServiceBus;

    public class AzureServiceBusTestHarness : BusTestHarness
    {
        private static readonly ILog _log = Logger.Get<AzureServiceBusTestHarness>();
        private readonly Uri _serviceUri;

        private Uri _inputQueueAddress;

        public AzureServiceBusTestHarness(Uri serviceUri, string sharedAccessKeyName, string sharedAccessKeyValue, string inputQueueName = null)
        {
            _serviceUri = serviceUri ?? throw new ArgumentNullException(nameof(serviceUri));
            SharedAccessKeyName = sharedAccessKeyName;
            SharedAccessKeyValue = sharedAccessKeyValue;

            TokenTimeToLive = TimeSpan.FromDays(1);
            TokenScope = TokenScope.Namespace;

            InputQueueName = inputQueueName ?? "input_queue";

            ServiceBusEnvironment.SystemConnectivity.Mode = ConnectivityMode.Https;
        }

        public event Action<IServiceBusBusFactoryConfigurator> OnConfigureServiceBusBus;

        public event Action<IServiceBusBusFactoryConfigurator, IServiceBusHost> OnConfigureServiceBusBusHost;

        public event Action<IServiceBusReceiveEndpointConfigurator> OnConfigureServiceBusReceiveEndpoint;

        public string SharedAccessKeyName { get; }

        public string SharedAccessKeyValue { get; }

        public TimeSpan TokenTimeToLive { get; set; }

        public TokenScope TokenScope { get; set; }

        public string InputQueueName { get; }

        public IServiceBusHost Host { get; private set; }

        public override Uri InputQueueAddress => _inputQueueAddress;

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
            OnConfigureServiceBusBus?.Invoke(configurator);
        }

        protected virtual void ConfigureServiceBusBusHost(IServiceBusBusFactoryConfigurator configurator, IServiceBusHost host)
        {
            OnConfigureServiceBusBusHost?.Invoke(configurator, host);
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
            OnConfigureServiceBusReceiveEndpoint?.Invoke(configurator);
        }

        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingAzureServiceBus(x =>
            {
                Host = x.Host(_serviceUri, h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = SharedAccessKeyName;
                        s.SharedAccessKey = SharedAccessKeyValue;
                        s.TokenTimeToLive = TokenTimeToLive;
                        s.TokenScope = TokenScope;
                    });
                });

                ConfigureBus(x);

                ConfigureServiceBusBus(x);

                x.UseServiceBusMessageScheduler();

                ConfigureServiceBusBusHost(x, Host);

                x.ReceiveEndpoint(Host, InputQueueName, e =>
                {
                    ConfigureReceiveEndpoint(e);

                    ConfigureServiceBusReceiveEndpoint(e);

                    _inputQueueAddress = e.InputAddress;
                });
            });
        }
    }
}