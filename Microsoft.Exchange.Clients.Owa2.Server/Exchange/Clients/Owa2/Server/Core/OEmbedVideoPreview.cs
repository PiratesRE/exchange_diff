using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OEmbedVideoPreview : LinkPreview
	{
		[DataMember]
		public string EmbeddedHtml { get; set; }
	}
}
