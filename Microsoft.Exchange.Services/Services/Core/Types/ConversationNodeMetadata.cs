using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ConversationNodeMetadata")]
	[Serializable]
	public class ConversationNodeMetadata
	{
		[DataMember(Name = "From", IsRequired = true, Order = 1)]
		public SingleRecipientType From { get; set; }

		[DataMember(Name = "Sender", IsRequired = true, Order = 2)]
		public SingleRecipientType Sender { get; set; }

		[DataMember(Name = "InternetMessageId", IsRequired = true, Order = 3)]
		public string InternetMessageId { get; set; }

		[DataMember(Name = "SubjectPrefix", IsRequired = true, Order = 4)]
		public string SubjectPrefix { get; set; }

		[DataMember(Name = "Preview", IsRequired = true, Order = 5)]
		public string Preview { get; set; }

		[DataMember(Name = "References", IsRequired = true, Order = 6)]
		public string References { get; set; }

		[DataMember(Name = "AddedParticipants", IsRequired = true, Order = 7)]
		public SingleRecipientType[] AddedParticipants { get; set; }

		[DataMember(Name = "ReplyAllParticipants", IsRequired = true, Order = 8)]
		public SingleRecipientType[] ReplyAllParticipants { get; set; }

		[DateTimeString]
		[DataMember(Name = "ReceivedTime", IsRequired = true, Order = 9)]
		public string ReceivedTime { get; set; }

		[DataMember(Name = "InReplyTo", IsRequired = true, Order = 10)]
		public string InReplyTo { get; set; }

		[DataMember(Name = "ConversationIndexTrackingEx", IsRequired = true, Order = 11)]
		public string ConversationIndexTrackingEx { get; set; }

		[DataMember(Name = "Order", IsRequired = true, Order = 12)]
		public int Order { get; set; }

		[DataMember(Name = "ItemClass", IsRequired = true, Order = 13)]
		public string ItemClass { get; set; }

		[DataMember(Name = "ThreadId", IsRequired = true, Order = 14)]
		public ItemId ThreadId { get; set; }

		[DataMember(Name = "Subject", IsRequired = true, Order = 15)]
		public string Subject { get; set; }
	}
}
