using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetWacIframeUrlForOneDriveRequest
	{
		[DataMember]
		public string EndPointUrl { get; set; }

		[DataMember]
		public string DocumentUrl { get; set; }

		[DataMember]
		public bool IsEdit { get; set; }
	}
}
