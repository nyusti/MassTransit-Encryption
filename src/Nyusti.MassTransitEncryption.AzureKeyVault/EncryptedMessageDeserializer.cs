namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using System.Net.Mime;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Serialization;
    using MassTransit.Util;
    using Microsoft.Azure.KeyVault.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// Encrypted message deserializer
    /// </summary>
    /// <seealso cref="MassTransit.IMessageDeserializer"/>
    /// <seealso cref="GreenPipes.IProbeSite"/>
    public class EncryptedMessageDeserializer : IMessageDeserializer, IProbeSite
    {
        private readonly JsonSerializer deserializer;
        private readonly IObjectTypeDeserializer objectTypeDeserializer;
        private readonly Func<IKeyResolver> keyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedMessageDeserializer"/> class.
        /// </summary>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <exception cref="ArgumentNullException">deserializer or keyResolver is null</exception>
        public EncryptedMessageDeserializer(JsonSerializer deserializer, IKeyResolver keyResolver)
            : this(deserializer, () => keyResolver)
        {
            if (keyResolver == null)
            {
                throw new ArgumentNullException(nameof(keyResolver));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedMessageDeserializer"/> class.
        /// </summary>
        /// <param name="deserializer">The deserializer.</param>
        /// <param name="keyResolver">The key resolver.</param>
        /// <exception cref="ArgumentNullException">keyResolver or deserializer</exception>
        public EncryptedMessageDeserializer(JsonSerializer deserializer, Func<IKeyResolver> keyResolver)
        {
            this.keyResolver = keyResolver ?? throw new ArgumentNullException(nameof(keyResolver));
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.objectTypeDeserializer = new ObjectTypeDeserializer(this.deserializer);
        }

        /// <inheritdoc/>
        public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Dispose method is overridden")]
        public ConsumeContext Deserialize(ReceiveContext receiveContext)
        {
            if (receiveContext == null)
            {
                throw new ArgumentNullException(nameof(receiveContext));
            }

            try
            {
                var encryptionDataJson = receiveContext.TransportHeaders.Get<string>(EncryptedMessageSerializer.EncryptionHeaderDataKey);
                var encryptionData = JsonConvert.DeserializeObject<EncryptionData>(encryptionDataJson);

                if (!encryptionData.EncryptionAgent.Protocol.Equals("1.0", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotSupportedException($"Version of {encryptionData?.EncryptionAgent?.Protocol ?? "(Not set)"} encryption agent is not supported.");
                }

                var resolver = this.keyResolver();

                var result = resolver.ResolveKeyAsync(encryptionData.WrappedContentKey.KeyId, receiveContext.CancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
                var key = result.UnwrapKeyAsync(encryptionData.WrappedContentKey.EncryptedKey, encryptionData.WrappedContentKey.Algorithm, receiveContext.CancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();

                MessageEnvelope envelope = null;
                if (encryptionData.EncryptionAgent.EncryptionAlgorithm == EncryptionAlgorithm.AES_CBC_256)
                {
                    using (var provider = new AesCryptoServiceProvider())
                    using (var transform = provider.CreateDecryptor(key, encryptionData.ContentEncryptionIV))
                    using (var cryptoStream = new DisposingCryptoStream(receiveContext.GetBody(), transform, CryptoStreamMode.Read))
                    using (var bsonReader = new BsonDataReader(cryptoStream))
                    {
                        envelope = this.deserializer.Deserialize<MessageEnvelope>(bsonReader);
                    }
                }
                else
                {
                    throw new NotSupportedException($"{encryptionData?.EncryptionAgent?.EncryptionAlgorithm} encryption algorithm detected.");
                }

                return new JsonConsumeContext(this.deserializer, this.objectTypeDeserializer, receiveContext, envelope);
            }
            catch (JsonSerializationException ex)
            {
                throw new SerializationException("A JSON serialization exception occurred while deserializing the message envelope", ex);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new SerializationException("An exception occurred while deserializing the message envelope", ex);
            }
        }

        /// <inheritdoc/>
        public void Probe(ProbeContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var scope = context.CreateScope("encrypted");
            scope.Add("contentType", EncryptedMessageSerializer.EncryptedContentType.MediaType);
        }
    }
}