namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using Microsoft.Azure.KeyVault.Core;

    /// <summary>
    /// Azure Key Vault encryption configuration
    /// </summary>
    public class AzureKeyVaultEncryptionConfigurator : IEncryptionConfigurator
    {
        /// <inheritdoc/>
        public Func<IKey> EncryptionKey { get; set; }

        /// <inheritdoc/>
        public Func<IKeyResolver> KeyResolver { get; set; }
    }
}