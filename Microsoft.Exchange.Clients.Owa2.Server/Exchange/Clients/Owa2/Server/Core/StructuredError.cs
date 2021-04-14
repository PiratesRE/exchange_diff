using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class StructuredError
	{
		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public string Info { get; set; }

		[DataMember]
		public string TargetSite { get; set; }

		[DataMember]
		public string Source { get; set; }

		[DataMember]
		public int Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Guid { get; set; }
	}
}
