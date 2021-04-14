using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class JsonFaultBody
	{
		[DataMember]
		public string FaultMessage { get; set; }

		[DataMember]
		public int ErrorCode { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string StackTrace { get; set; }
	}
}
