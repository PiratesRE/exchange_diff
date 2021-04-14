using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(OEmbedVideoPreview))]
	[KnownType(typeof(YouTubeLinkPreview))]
	[KnownType(typeof(LinkPreview))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public abstract class BaseLinkPreview
	{
		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public long RequestStartTimeMilliseconds { get; set; }
	}
}
