namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using Microsoft.Azure.KeyVault.Core;

    /// <summary>
    /// Encryption configurator
    /// </summary>
    public interface IEncryptionConfigurator
    {
        /// <summary>
        /// Gets or sets the encryption key. This key is needed for message encryption. If set the
        /// messages will be encrypted with this key.
        /// </summary>
        /// <value>The encryption key.</value>
        Func<IKey> EncryptionKey { get; set; }

        /// <summary>
        /// Gets or sets the key resolver. The key resolver will be used to locate the decryption key based on
        /// the message metadata. This has to be set for message decryption.
        /// </summary>
        /// <value>The key resolver.</value>
        Func<IKeyResolver> KeyResolver { get; set; }
    }
}