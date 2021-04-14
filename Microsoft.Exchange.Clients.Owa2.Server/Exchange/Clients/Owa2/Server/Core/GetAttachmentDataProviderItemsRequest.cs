using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetAttachmentDataProviderItemsRequest
	{
		[DataMember]
		public string AttachmentDataProviderId { get; set; }

		[DataMember]
		public AttachmentItemsPagingDetails Paging { get; set; }

		[DataMember]
		public AttachmentDataProviderScope Scope { get; set; }
	}
}
