using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	internal class SearchNotificationPayload : NotificationPayloadBase
	{
		public SearchNotificationPayload()
		{
			base.SubscriptionId = NotificationType.SearchNotification.ToString();
		}

		[DataMember]
		public string FolderId { get; set; }

		[DataMember]
		public string ClientId { get; set; }

		[DataMember]
		public bool IsComplete { get; set; }

		[DataMember]
		public ItemType[] MessageItems { get; set; }

		[DataMember]
		public ConversationType[] Conversations { get; set; }

		[DataMember]
		public int ServerSearchResultsRowCount { get; set; }

		[DataMember]
		public HighlightTermType[] HighlightTerms { get; set; }
	}
}
