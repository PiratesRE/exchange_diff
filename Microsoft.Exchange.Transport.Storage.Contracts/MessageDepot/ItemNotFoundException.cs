using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	[Serializable]
	internal class ItemNotFoundException : MessageDepotPermanentException
	{
		public ItemNotFoundException(TransportMessageId messageId, LocalizedString errorMessage, Exception innerException = null) : base(errorMessage, innerException)
		{
			if (messageId == null)
			{
				throw new ArgumentNullException("messageId");
			}
			this.messageId = messageId;
		}

		protected ItemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.messageId = (TransportMessageId)info.GetValue("messageId", typeof(TransportMessageId));
		}

		public TransportMessageId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("messageId", this.messageId);
		}

		private const string MessageIdSerializedName = "messageId";

		private readonly TransportMessageId messageId;
	}
}
