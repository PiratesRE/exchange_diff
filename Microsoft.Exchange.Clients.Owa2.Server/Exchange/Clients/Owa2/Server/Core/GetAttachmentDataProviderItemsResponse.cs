using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class GetAttachmentDataProviderItemsResponse
	{
		[DataMember]
		public AttachmentResultCode ResultCode { get; set; }

		[DataMember]
		public AttachmentDataProviderItem[] Items { get; set; }

		[DataMember]
		public AttachmentItemsPagingMetadata PagingMetadata { get; set; }

		[DataMember]
		public int TotalItemCount { get; set; }
	}
}
