using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuiteStorageKeyType
	{
		[DataMember]
		public string Scope { get; set; }

		[DataMember]
		public string Namespace { get; set; }

		[DataMember]
		public string Name { get; set; }
	}
}
