using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SuiteStorageRequest
	{
		[DataMember]
		public SuiteStorageKeyType[] ReadSettings { get; set; }

		[DataMember]
		public SuiteStorageType[] WriteSettings { get; set; }
	}
}
