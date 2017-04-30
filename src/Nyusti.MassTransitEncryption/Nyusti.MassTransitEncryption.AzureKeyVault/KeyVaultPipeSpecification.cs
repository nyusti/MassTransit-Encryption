namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

            builder.SetMessageSerializer(new SerializerFactory(this.GetMessageSerializer));
            builder.AddMessageDeserializer(EncryptedMessageSerializer.EncryptedContentType, new DeserializerFactory(this.GetMessageDeserializer));
        }

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate()
        {
            return Enumerable.Empty<ValidationResult>();
        }

        private IMessageSerializer GetMessageSerializer()
        {
            if (this.encryptionKey == null)
            {
                throw new InvalidOperationException("Encryption key must be set to support message sending.");
            }

            return new EncryptedMessageSerializer(this.encryptionKey());
        }

        private IMessageDeserializer GetMessageDeserializer()
        {
            if (this.keyResolver == null)
            {
                throw new InvalidOperationException("Decryption key resolver must be set to support message receiving.");
            }

            return new EncryptedMessageDeserializer(BsonMessageSerializer.Deserializer, this.keyResolver());
        }
    }
}