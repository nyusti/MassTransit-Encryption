namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using System.Collections.Generic;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Builders;
    using MassTransit.Serialization;
    using Microsoft.Azure.KeyVault.Core;

    /// <summary>
    /// Key vault encyrption pipe specification
    /// </summary>
    /// <seealso cref="MassTransit.Builders.IBusFactorySpecification"/>
    /// <seealso cref="GreenPipes.ISpecification"/>
    internal class KeyVaultPipeSpecification : IBusFactorySpecification, ISpecification
    {
        private readonly Func<IKey> encryptionKey;
        private readonly Func<IKeyResolver> keyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyVaultPipeSpecification"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <param name="keyResolver">The key resolver.</param>
        public KeyVaultPipeSpecification(Func<IKey> encryptionKey, Func<IKeyResolver> keyResolver)
        {
            this.encryptionKey = encryptionKey;
            this.keyResolver = keyResolver;
        }

        /// <inheritdoc/>
        public void Apply(IBusBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (this.encryptionKey != null)
            {
                builder.SetMessageSerializer(this.GetMessageSerializer);
            }

            if (this.keyResolver != null)
            {
                builder.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType, this.GetMessageDeserializer);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate()
        {
            if (this.encryptionKey == null && this.keyResolver == null)
            {
                yield return ValidationResultExtensions.Failure(this, "No encryption key or key resolver factory set!");
            }

            if (this.encryptionKey != null)
            {
                yield return ValidationResultExtensions.Success(this, "Encryption key factory set.");

                if (this.keyResolver == null)
                {
                    yield return ValidationResultExtensions.Warning(this, "Key resolver factory is not set. Only encryption will be supported.");
                }
            }

            if (this.keyResolver != null)
            {
                yield return ValidationResultExtensions.Success(this, "Key resolver factory set.");

                if (this.encryptionKey == null)
                {
                    yield return ValidationResultExtensions.Warning(this, "Encryption key factory is not set. Only decryption will be supported.");
                }
            }
        }

        private IMessageSerializer GetMessageSerializer()
        {
            return new EncryptedMessageSerializer(this.encryptionKey);
        }

        private IMessageDeserializer GetMessageDeserializer()
        {
            return new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, this.keyResolver);
        }
    }
}