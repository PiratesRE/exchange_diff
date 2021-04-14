using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	public class JsonFaultBody
	{
		[DataMember(IsRequired = true)]
		public string FaultMessage { get; set; }

		[DataMember(IsRequired = true)]
		public int ErrorCode { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string StackTrace { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ResponseCode { get; set; }

		[DataMember]
		public bool IsTransient { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int BackOffPeriodInMs { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string ExceptionName { get; set; }
	}
}
