using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CalendarActionResponse
	{
		public CalendarActionResponse(CalendarActionError errorCode)
		{
			this.WasSuccessful = false;
			this.ErrorCode = errorCode;
		}

		public CalendarActionResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public CalendarActionError ErrorCode { get; set; }
	}
}
