using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SendLinkClickedSignalToSPRequest
	{
		[DataMember]
		public ItemId ID { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string ImgURL { get; set; }

		[DataMember]
		public string ImgDimensions { get; set; }
	}
}
