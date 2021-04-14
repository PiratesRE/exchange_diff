using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	[Serializable]
	public class FlowConversationItem
	{
		[DataMember]
		public string FlowConversationId { get; set; }

		[DataMember]
		public string SenderPhotoEmailAddress { get; set; }

		[DataMember]
		public string Preview { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public int UnReadCount { get; set; }

		[DataMember]
		public string ReceivedTimeUtc { get; set; }

		[DataMember]
		public ItemId LastItemId { get; set; }

		[DataMember]
		public EmailAddressWrapper[] Participants { get; set; }

		[DataMember]
		public int TotalCount { get; set; }
	}
}
