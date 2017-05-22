namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using System.Linq;
    using MassTransit;
    using MassTransit.RabbitMqTransport;
    using MassTransit.Testing;
    using MassTransit.Transports;
    using RabbitMQ.Client;

    /// <summary>
    /// RabbitMQ test harness
    /// </summary>
    /// <seealso cref="MassTransit.Testing.BusTestHarness"/>
    public class RabbitMqTestHarness : BusTestHarness
    {
        /// <summary>
        /// The host address
        /// </summary>
        private Uri hostAddress;

        /// <summary>
        /// The input queue address
        /// </summary>
        private Uri inputQueueAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqTestHarness"/> class.
        /// </summary>
        /// <param name="inputQueueName">Name of the input queue.</param>
        public RabbitMqTestHarness(string inputQueueName = null)
        {
            this.Username = "guest";
            this.Password = "guest";

            this.InputQueueName = inputQueueName ?? "input_queue";

            this.NameFormatter = new RabbitMqMessageNameFormatter();

            this.HostAddress = new Uri("rabbitmq://[::1]/");
        }

        /// <summary>
        /// Gets or sets the on configure rabbit mq bus.
        /// </summary>
        /// <value>The on configure rabbit mq bus.</value>
        public Action<IRabbitMqBusFactoryConfigurator> OnConfigureRabbitMqBus { get; set; }

        /// <summary>
        /// Gets or sets the on configure rabbit mq bus host.
        /// </summary>
        /// <value>The on configure rabbit mq bus host.</value>
        public Action<IRabbitMqBusFactoryConfigurator, IRabbitMqHost> OnConfigureRabbitMqBusHost { get; set; }

        /// <summary>
        /// Gets or sets the on configure rabbit mq receive endoint.
        /// </summary>
        /// <value>The on configure rabbit mq receive endoint.</value>
        public Action<IRabbitMqReceiveEndpointConfigurator> OnConfigureRabbitMqReceiveEndoint { get; set; }

        /// <summary>
        /// Gets or sets the on configure rabbit mq host.
        /// </summary>
        /// <value>The on configure rabbit mq host.</value>
        public Action<IRabbitMqHostConfigurator> OnConfigureRabbitMqHost { get; set; }

        /// <summary>
        /// Gets or sets the on cleanup virtual host.
        /// </summary>
        /// <value>The on cleanup virtual host.</value>
        public Action<IModel> OnCleanupVirtualHost { get; set; }

        /// <summary>
        /// Gets or sets the host address.
        /// </summary>
        /// <value>The host address.</value>
        public Uri HostAddress
        {
            get
            {
                return this.hostAddress;
            }

            set
            {
                this.hostAddress = value;
                this.inputQueueAddress = new Uri(this.HostAddress, this.InputQueueName);
            }
        }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets the name of the input queue.
        /// </summary>
        /// <value>The name of the input queue.</value>
        public string InputQueueName { get; }

        /// <summary>
        /// Gets or sets the name of the node host.
        /// </summary>
        /// <value>The name of the node host.</value>
        public string NodeHostName { get; set; }

        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public IRabbitMqHost Host { get; private set; }

        /// <summary>
        /// Gets the name formatter.
        /// </summary>
        /// <value>The name formatter.</value>
        public IMessageNameFormatter NameFormatter { get; }

        /// <inheritdoc/>
        public override Uri InputQueueAddress => this.inputQueueAddress;

        /// <summary>
        /// Configures the rabbit mq bus.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
            this.OnConfigureRabbitMqBus?.Invoke(configurator);
        }

        /// <summary>
        /// Configures the rabbit mq bus host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="host">The host.</param>
        protected virtual void ConfigureRabbitMqBusHost(IRabbitMqBusFactoryConfigurator configurator, IRabbitMqHost host)
        {
            this.OnConfigureRabbitMqBusHost?.Invoke(configurator, host);
        }

        /// <summary>
        /// Configures the rabbit mq receive endpoint.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
            this.OnConfigureRabbitMqReceiveEndoint?.Invoke(configurator);
        }

        /// <summary>
        /// Configures the rabbit mq host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
            this.OnConfigureRabbitMqHost?.Invoke(configurator);
        }

        /// <summary>
        /// Cleanups the virtual host.
        /// </summary>
        /// <param name="model">The model.</param>
        protected virtual void CleanupVirtualHost(IModel model)
        {
            this.OnCleanupVirtualHost?.Invoke(model);
        }

        /// <summary>
        /// Configures the host.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <returns>The host.</returns>
        protected virtual IRabbitMqHost ConfigureHost(IRabbitMqBusFactoryConfigurator configurator)
        {
            return configurator.Host(this.HostAddress, h =>
            {
                h.Username(this.Username);
                h.Password(this.Password);

                if (!string.IsNullOrWhiteSpace(this.NodeHostName))
                {
                    h.UseCluster(c => c.Node(this.NodeHostName));
                }

                this.ConfigureRabbitMqHost(h);
            });
        }

        /// <inheritdoc/>
        protected override IBusControl CreateBus()
        {
            return MassTransit.Bus.Factory.CreateUsingRabbitMq(x =>
            {
                this.Host = this.ConfigureHost(x);
                this.CleanUpVirtualHost(this.Host);
                this.ConfigureBus(x);
                this.ConfigureRabbitMqBus(x);
                this.ConfigureRabbitMqBusHost(x, this.Host);

                x.ReceiveEndpoint(this.Host, this.InputQueueName, e =>
                {
                    e.PrefetchCount = 16;
                    e.PurgeOnStartup = true;

                    this.ConfigureReceiveEndpoint(e);
                    this.ConfigureRabbitMqReceiveEndpoint(e);

                    this.inputQueueAddress = e.InputAddress;
                });
            });
        }

        /// <summary>
        /// Cleans up virtual host.
        /// </summary>
        /// <param name="host">The host.</param>
        private void CleanUpVirtualHost(IRabbitMqHost host)
        {
            try
            {
                var connectionFactory = host.Settings.GetConnectionFactory();
                using (
                    var connection = host.Settings.ClusterMembers?.Any() ?? false
                        ? connectionFactory.CreateConnection(host.Settings.ClusterMembers, host.Settings.Host)
                        : connectionFactory.CreateConnection())
                using (var model = connection.CreateModel())
                {
                    model.ExchangeDelete("input_queue");
                    model.QueueDelete("input_queue");

                    model.ExchangeDelete("input_queue_skipped");
                    model.QueueDelete("input_queue_skipped");

                    model.ExchangeDelete("input_queue_error");
                    model.QueueDelete("input_queue_error");

                    model.ExchangeDelete("input_queue_delay");
                    model.QueueDelete("input_queue_delay");

                    model.ExchangeDelete(this.InputQueueName);
                    model.QueueDelete(this.InputQueueName);

                    model.ExchangeDelete(this.InputQueueName + "_skipped");
                    model.QueueDelete(this.InputQueueName + "_skipped");

                    model.ExchangeDelete(this.InputQueueName + "_error");
                    model.QueueDelete(this.InputQueueName + "_error");

                    model.ExchangeDelete(this.InputQueueName + "_delay");
                    model.QueueDelete(this.InputQueueName + "_delay");

                    this.CleanupVirtualHost(model);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}