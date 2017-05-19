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
        /// Returns the CorrelationId for the message
        /// </summary>
        public Guid CorrelationId => this.id;

        /// <summary>
        /// Equalses the specified object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
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

            return obj.id.Equals(id);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/>, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance;
        /// otherwise, <c>false</c>.
        /// </returns>
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

            return Equals((PingMessage)obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures
        /// like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }
}