using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetLinkPreviewRequest
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public long RequestStartTimeMilliseconds { get; set; }

		[DataMember]
		public bool Autoplay { get; set; }
	}
}
