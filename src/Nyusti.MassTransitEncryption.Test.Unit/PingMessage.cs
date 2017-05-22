namespace Nyusti.MassTransitEncryption.Test.Unit
{
    using System;
    using MassTransit;

    /// <summary>
    /// Ping message
    /// </summary>
    [Serializable]
    public class PingMessage : IEquatable<PingMessage>, CorrelatedBy<Guid>
    {
        /// <summary>
        /// The identifier
        /// </summary>
        private Guid id = new Guid("D62C9B1C-8E31-4D54-ADD7-C624D56085A4");

        /// <summary>
        /// Initializes a new instance of the <see cref="PingMessage"/> class.
        /// </summary>
        public PingMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PingMessage"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public PingMessage(Guid id)
        {
            this.id = id;
        }

        /// <summary>
        /// Gets returns the CorrelationId for the message
        /// </summary>
        public Guid CorrelationId => this.id;

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True if equals</returns>
        public bool Equals(PingMessage obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return obj.id.Equals(this.id);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(PingMessage))
            {
                return false;
            }

            return this.Equals((PingMessage)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}