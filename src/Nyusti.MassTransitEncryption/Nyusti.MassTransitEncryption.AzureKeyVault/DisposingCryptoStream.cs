namespace Nyusti.MassTransitEncryption.AzureKeyVault
{
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Disposing crypto stream
    /// </summary>
    /// <seealso cref="System.Security.Cryptography.CryptoStream"/>
    internal class DisposingCryptoStream : CryptoStream
    {
        /// <summary>
        /// The stream
        /// </summary>
        private Stream stream;

        /// <summary>
        /// The transform
        /// </summary>
        private ICryptoTransform transform;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposingCryptoStream"/> class.
        /// </summary>
        /// <param name="stream">The stream on which to perform the cryptographic transformation.</param>
        /// <param name="transform">
        /// The cryptographic transformation that is to be performed on the stream.
        /// </param>
        /// <param name="mode">
        /// One of the <see cref="T:System.Security.Cryptography.CryptoStreamMode"/> values.
        /// </param>
        public DisposingCryptoStream(Stream stream, ICryptoTransform transform, CryptoStreamMode mode)
          : base(stream, transform, mode)
        {
            this.stream = stream;
            this.transform = transform;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose", Justification = "Dispose only if the child disposes")]
        protected override void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            base.Dispose(true);

            this.stream?.Dispose();
            this.stream = null;

            this.transform?.Dispose();
            this.transform = null;
        }
    }
}