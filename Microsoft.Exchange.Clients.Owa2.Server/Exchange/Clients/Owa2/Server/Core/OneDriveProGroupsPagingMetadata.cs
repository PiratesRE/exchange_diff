using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OneDriveProGroupsPagingMetadata : OneDriveProItemsPagingMetadata
	{
		[DataMember]
		public string GroupSmtpAddress { get; set; }

		[DataMember]
		public string GroupEndpointUrl { get; set; }
	}
}
