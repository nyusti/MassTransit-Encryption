namespace MassTransit
{
    using System;
    using Microsoft.Azure.KeyVault.Core;
    using Nyusti.MassTransitEncryption.AzureKeyVault;

    /// <summary>
    /// Serializer configuration extensions
    /// </summary>
    public static class SerializerConfigurationExtensions
    {
        /// <summary>
        /// Uses the azure key vault encryption. Encryption key must be set for outgoing message
        /// encryption. The key resolver must be set for message decryption.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <exception cref="ArgumentNullException">configurator is null</exception>
        /// <exception cref="ArgumentException">encryptionKey and keyResolver are null</exception>
        public static void UseAzureKeyVaultEncryption(this IBusFactoryConfigurator configurator, IKey encryptionKey, IKeyResolver keyResolver)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (encryptionKey == null && keyResolver == null)
            {
                throw new ArgumentException($"{nameof(encryptionKey)} or {nameof(keyResolver)} must be set.");
            }

            configurator.AddBusFactorySpecification(new KeyVaultPipeSpecification(() => encryptionKey, () => keyResolver));
        }

        /// <summary>
        /// Uses the azure key vault encryption. Encryption key must be set for outgoing message
        /// encryption. The key resolver must be set for message decryption.
        /// </summary>
        /// <param name="configurator">The configurator.</param>
        /// <param name="encryptionConfigurator">The encryption configurator.</param>
        /// <exception cref="System.ArgumentNullException">
        /// configurator or encryptionConfigurator is null
        /// </exception>
        public static void UseAzureKeyVaultEncryption(this IBusFactoryConfigurator configurator, Action<IEncryptionConfigurator> encryptionConfigurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException(nameof(configurator));
            }

            if (encryptionConfigurator == null)
            {
                throw new ArgumentNullException(nameof(encryptionConfigurator));
            }

            var configuration = new AzureKeyVaultEncryptionConfigurator();
            encryptionConfigurator(configuration);

            configurator.AddBusFactorySpecification(new KeyVaultPipeSpecification(configuration.EncryptionKey, configuration.KeyResolver));
        }
    }
}