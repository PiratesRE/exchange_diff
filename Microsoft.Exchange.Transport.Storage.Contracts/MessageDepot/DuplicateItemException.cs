using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.MessageDepot
{
	[Serializable]
	internal class DuplicateItemException : MessageDepotPermanentException
	{
		public DuplicateItemException(TransportMessageId messageId, MessageDepotItemState messageState, LocalizedString errorMessage, Exception innerException = null) : base(errorMessage, innerException)
		{
			if (messageId == null)
			{
				throw new ArgumentNullException("messageId");
			}
			this.messageId = messageId;
			this.messageState = messageState;
		}

		protected DuplicateItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.messageId = (TransportMessageId)info.GetValue("messageId", typeof(TransportMessageId));
			this.messageState = (MessageDepotItemState)info.GetInt32("messageState");
		}

		public TransportMessageId MessageId
		{
			get
			{
				return this.messageId;
			}
		}

		public MessageDepotItemState MessageState
		{
			get
			{
				return this.messageState;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("messageId", this.messageId);
			info.AddValue("messageState", this.messageState);
		}

		private const string MessageIdSerializedName = "messageId";

		private const string MessageStateSerializedName = "messageState";

		private readonly TransportMessageId messageId;

		private readonly MessageDepotItemState messageState;
	}
}
