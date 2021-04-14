using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetModernAttachmentsRequest
	{
		public GetModernAttachmentsRequest()
		{
			this.ItemCreationTimeStart = null;
			this.ItemCreationTimeEnd = null;
			this.ItemFromStart = null;
			this.ItemFromEnd = null;
		}

		[DataMember]
		public int AttachmentsReturnedMax { get; set; }

		[DataMember]
		public int ItemsToProcessMax { get; set; }

		[DataMember]
		public int ItemsOffset { get; set; }

		[DataMember]
		public string ItemCreationTimeStart { get; set; }

		[DataMember]
		public string ItemCreationTimeEnd { get; set; }

		[DataMember]
		public string ItemFromStart { get; set; }

		[DataMember]
		public string ItemFromEnd { get; set; }

		[DataMember]
		public string[] Filter { get; set; }

		[DataMember]
		public GetModernAttachmentsRequest.AttachmentsSortBy[] SortByColumns { get; set; }

		[DataMember]
		public BaseFolderId[] FolderId { get; set; }

		[DataMember]
		public BaseItemId[] ItemId { get; set; }

		[Flags]
		public enum AttachmentsSortBy
		{
			ItemConversationTopic = 1,
			ItemClass = 2,
			ItemSubject = 3,
			ItemReceivedTime = 4,
			ItemCreationTime = 5,
			ItemLastModifiedTime = 6,
			ItemSize = 7,
			ItemSentRepresentingDisplayName = 8,
			DescendingOrder = 256
		}
	}
}
