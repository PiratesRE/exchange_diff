using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class InlineExploreSpResultListType
	{
		[DataMember]
		public int ResultCount { get; set; }

		[DataMember]
		public InlineExploreSpResultItemType[] ResultItems { get; set; }

		[DataMember]
		public string Status { get; set; }
	}
}
