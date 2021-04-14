using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InlineExploreSpResultItemType
	{
		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Url { get; set; }

		[DataMember]
		public string FileType { get; set; }

		[DataMember]
		public string LastModifiedTime { get; set; }

		[DataMember]
		public string Summary { get; set; }
	}
}
