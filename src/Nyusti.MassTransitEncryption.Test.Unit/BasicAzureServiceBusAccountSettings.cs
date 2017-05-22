namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.ServiceBus;

    /// <summary>
    /// Azure service bus connection settings
    /// </summary>
    /// <seealso cref="MassTransit.AzureServiceBusTransport.ServiceBusTokenProviderSettings"/>
    public sealed class BasicAzureServiceBusAccountSettings : ServiceBusTokenProviderSettings
    {
        /// <summary>
        /// The key name
        /// </summary>
        private const string KeyName = "RootManageSharedAccessKey";

        /// <summary>
        /// The shared access key
        /// </summary>
        private const string SharedAccessKey = "secret";

        /// <summary>
        /// The token scope
        /// </summary>
        private readonly TokenScope tokenScope;

        /// <summary>
        /// The token time to live
        /// </summary>
        private readonly TimeSpan tokenTimeToLive;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAzureServiceBusAccountSettings"/> class.
        /// </summary>
        public BasicAzureServiceBusAccountSettings()
        {
            this.tokenTimeToLive = TimeSpan.FromDays(1);
            this.tokenScope = TokenScope.Namespace;
        }

        /// <summary>
        /// Gets the name of the key.
        /// </summary>
        /// <value>The name of the key.</value>
        string ServiceBusTokenProviderSettings.KeyName { get; } = KeyName;

        /// <summary>
        /// Gets the shared access key.
        /// </summary>
        /// <value>The shared access key.</value>
        string ServiceBusTokenProviderSettings.SharedAccessKey { get; } = SharedAccessKey;

        /// <summary>
        /// Gets the token time to live.
        /// </summary>
        /// <value>The token time to live.</value>
        TimeSpan ServiceBusTokenProviderSettings.TokenTimeToLive
        {
            get
            {
                return this.tokenTimeToLive;
            }
        }

        /// <summary>
        /// Gets the token scope.
        /// </summary>
        /// <value>The token scope.</value>
        TokenScope ServiceBusTokenProviderSettings.TokenScope
        {
            get
            {
                return this.tokenScope;
            }
        }
    }
}