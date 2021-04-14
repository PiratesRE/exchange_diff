using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AttachmentItemsPagingDetails
	{
		[DataMember]
		public IndexedPageView RequestedData { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public AttachmentItemsSort Sort { get; set; }

		[DataMember]
		public AttachmentItemsPagingMetadata PagingMetadata { get; set; }

		[DataMember]
		public string ItemsEndpointUrl { get; set; }
	}
}
