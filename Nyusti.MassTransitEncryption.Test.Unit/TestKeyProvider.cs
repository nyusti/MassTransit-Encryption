namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.KeyVault.Core;
    using NSubstitute;
    using Nyusti.MassTransitEncryption.AzureKeyVault;

    public class TestKeyProvider : IEncryptionConfigurator
    {
        public TestKeyProvider()
        {
            const string testKeyId = "TestKey";
            var cert = new X509Certificate2(@"c:\Dealogic\CN\CNTrunk\products\Portal\installer\src\Dealogic.Connect.Portal.Installer\Dealogic.Connect.Portal.Installer\certificates\connect-encryption.pfx", "De@l123", X509KeyStorageFlags.Exportable);

            var key = new RsaKey(testKeyId, (RSACryptoServiceProvider)cert.PrivateKey);

            var keyResolver = Substitute.For<IKeyResolver>();
            keyResolver.ResolveKeyAsync(testKeyId, Arg.Any<CancellationToken>()).Returns(key);

            this.EncryptionKey = () => key;
            this.KeyResolver = () => keyResolver;
        }

        public Func<IKey> EncryptionKey { get; set; }

        public Func<IKeyResolver> KeyResolver { get; set; }
    }
}