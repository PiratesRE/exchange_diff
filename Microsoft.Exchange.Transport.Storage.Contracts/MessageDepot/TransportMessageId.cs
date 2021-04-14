using System;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	[Serializable]
	internal sealed class TransportMessageId : IEquatable<TransportMessageId>
	{
		public TransportMessageId(string messageId)
		{
			if (string.IsNullOrEmpty(messageId))
			{
				throw new ArgumentNullException("messageId");
			}
			this.messageId = messageId;
		}

		public string MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public static bool operator ==(TransportMessageId obj1, TransportMessageId obj2)
		{
			return object.ReferenceEquals(obj1, obj2) || (!object.ReferenceEquals(obj1, null) && !object.ReferenceEquals(obj2, null) && string.Equals(obj1.MessageId, obj2.MessageId));
		}

		public static bool operator !=(TransportMessageId obj1, TransportMessageId obj2)
		{
			return !(obj1 == obj2);
		}

		public bool Equals(TransportMessageId other)
		{
			return this == other;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as TransportMessageId);
		}

		public override string ToString()
		{
			return this.messageId;
		}

		public override int GetHashCode()
		{
			return this.messageId.GetHashCode();
		}

		private readonly string messageId;
	}
}
