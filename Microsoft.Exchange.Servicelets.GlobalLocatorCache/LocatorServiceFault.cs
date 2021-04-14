using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public class LocatorServiceFault
	{
		[DataMember]
		public ErrorCode ErrorCode { get; set; }

		[DataMember]
		public string Message { get; set; }

		[DataMember]
		public bool CanRetry { get; set; }

		public override string ToString()
		{
			return string.Format("ErrorCode: <{0}>, Message: <{1}>, CanRetry: <{2}>", this.ErrorCode, this.Message, this.CanRetry);
		}
	}
}
