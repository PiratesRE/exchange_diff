using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LinkPreview : BaseLinkPreview
	{
		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string ImageUrl { get; set; }

		[DataMember]
		public bool IsVideo { get; set; }
	}
}
