namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System;
    using System.IO;
    using System.Net.Mime;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.Serialization;
    using MassTransit.Util;
    using Microsoft.Azure.KeyVault.Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    /// <summary>
    /// Encrypted message serializer
    /// </summary>
    /// <seealso cref="IMessageSerializer"/>
    public class EncryptedMessageSerializer : IMessageSerializer
    {
        /// <summary>
        /// The encryption data key for message transport header
        /// </summary>
        public const string EncryptionHeaderDataKey = "encryptiondata";

        /// <summary>
        /// The content type header value
        /// </summary>
        public const string ContentTypeHeaderValue = "application/vnd.masstransit+aeskv";

        private static readonly ContentType EncryptedContentTypeValue = new ContentType(ContentTypeHeaderValue);

        private readonly JsonSerializer serializer;
        private readonly Func<IKey> encryptionKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedMessageSerializer"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <exception cref="ArgumentNullException">encryptionKey is null</exception>
        public EncryptedMessageSerializer(IKey encryptionKey)
            : this(() => encryptionKey)
        {
            if (encryptionKey == null)
            {
                throw new ArgumentNullException(nameof(encryptionKey));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EncryptedMessageSerializer"/> class.
        /// </summary>
        /// <param name="encryptionKey">The encryption key.</param>
        /// <exception cref="ArgumentNullException">encryptionKey is null</exception>
        public EncryptedMessageSerializer(Func<IKey> encryptionKey)
        {
            this.encryptionKey = encryptionKey ?? throw new ArgumentNullException(nameof(encryptionKey));
            this.serializer = BsonMessageSerializer.Serializer;
        }

        /// <summary>
        /// Gets the type of the encrypted content.
        /// </summary>
        /// <value>The type of the encrypted content.</value>
        public static ContentType EncryptedContentType => EncryptedContentTypeValue;

        /// <inheritdoc/>
        public ContentType ContentType => EncryptedMessageSerializer.EncryptedContentType;

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Dispose method is overridden")]
        public void Serialize<T>(Stream stream, SendContext<T> context)
            where T : class
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.ContentType = EncryptedMessageSerializer.EncryptedContentType;
            var jsonMessageEnvelope = new JsonMessageEnvelope(context, context.Message, TypeMetadataCache<T>.MessageTypeNames);
            using (var transform = this.CreateAndSetEncryptionContext(context.Headers, context.CancellationToken).ConfigureAwait(false).GetAwaiter().GetResult())
            using (var encryptStream = new DisposingCryptoStream(stream, transform, CryptoStreamMode.Write))
            using (var bsonWriter = new BsonDataWriter(encryptStream))
            {
                this.serializer.Serialize(bsonWriter, jsonMessageEnvelope, typeof(MessageEnvelope));
                bsonWriter.Flush();
            }
        }

        private async Task<ICryptoTransform> CreateAndSetEncryptionContext(SendHeaders metadata, CancellationToken cancellationToken)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (this.encryptionKey == null)
            {
                throw new InvalidOperationException("Key is not initialized. Encryption requires it to be initialized.");
            }

            using (AesCryptoServiceProvider provider = new AesCryptoServiceProvider())
            {
                var data = new EncryptionData
                {
                    EncryptionAgent = new EncryptionAgent("1.0", EncryptionAlgorithm.AES_CBC_256)
                };

                var key = this.encryptionKey();

                var result = await key.WrapKeyAsync(provider.Key, null, cancellationToken).ConfigureAwait(false);
                data.WrappedContentKey = new WrappedKey(key.Kid, result.Item1, result.Item2);
                data.ContentEncryptionIV = provider.IV;
                metadata.Set(EncryptionHeaderDataKey, JsonConvert.SerializeObject(data));
                return provider.CreateEncryptor();
            }
        }
    }
}