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

    /// <summary>
    /// Test key provider for unit tests
    /// </summary>
    /// <seealso cref="Nyusti.MassTransitEncryption.AzureKeyVault.IEncryptionConfigurator"/>
    public class TestKeyProvider : IEncryptionConfigurator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestKeyProvider"/> class.
        /// </summary>
        public TestKeyProvider()
        {
            const string testKeyId = "TestKey";
            var cert = new X509Certificate2(@"encryption.pfx", "password", X509KeyStorageFlags.Exportable);

            var key = new RsaKey(testKeyId, (RSACryptoServiceProvider)cert.PrivateKey);

            var keyResolver = Substitute.For<IKeyResolver>();
            keyResolver.ResolveKeyAsync(testKeyId, Arg.Any<CancellationToken>()).Returns(key);

            this.EncryptionKey = () => key;
            this.KeyResolver = () => keyResolver;
        }

        /// <summary>
        /// Gets or sets the encryption key. This key is needed for message encryption. If set the
        /// messages will be encrypted with this key.
        /// </summary>
        /// <value>The encryption key.</value>
        public Func<IKey> EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets the key resolver. The key resolver will be used to locate the decryption key
        /// based on the message metadata. This has to be set for message decryption.
        /// </summary>
        /// <value>The key resolver.</value>
        public Func<IKeyResolver> KeyResolver { get; set; }
    }
}