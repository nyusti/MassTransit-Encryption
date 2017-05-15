namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit.AzureServiceBusTransport;
    using Microsoft.ServiceBus;

    public class TestAzureServiceBusAccountSettings : ServiceBusTokenProviderSettings
    {
        private const string KeyName = "MassTransitBuild";
        private const string SharedAccessKey = "xsvaZOKYkX8JI5N+spLCkI9iu102jLhWFJrf0LmNPMw=";
        private readonly TokenScope _tokenScope;
        private readonly TimeSpan _tokenTimeToLive;

        public TestAzureServiceBusAccountSettings()
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