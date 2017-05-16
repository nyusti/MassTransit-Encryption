namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.ServiceBus;

    public class BasicAzureServiceBusAccountSettings : ServiceBusTokenProviderSettings
    {
        private const string KeyName = "RootManageSharedAccessKey";
        private const string SharedAccessKey = "+6AqGOJF2RJXtlGLYZu7ru4jcibFruPnWOMSLIl8o40=";
        private readonly TokenScope _tokenScope;
        private readonly TimeSpan _tokenTimeToLive;

        public BasicAzureServiceBusAccountSettings()
        {
            _tokenTimeToLive = TimeSpan.FromDays(1);
            _tokenScope = TokenScope.Namespace;
        }

        string ServiceBusTokenProviderSettings.KeyName
        {
            get { return KeyName; }
        }

        string ServiceBusTokenProviderSettings.SharedAccessKey
        {
            get { return SharedAccessKey; }
        }

        TimeSpan ServiceBusTokenProviderSettings.TokenTimeToLive
        {
            get { return _tokenTimeToLive; }
        }

        TokenScope ServiceBusTokenProviderSettings.TokenScope
        {
            get { return _tokenScope; }
        }
    }
}