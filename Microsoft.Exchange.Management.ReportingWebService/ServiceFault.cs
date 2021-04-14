using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	[DataContract]
	public class ServiceFault
	{
		public ServiceFault(string errorCode, Exception ex)
		{
			this.ErrorCode = errorCode;
			this.Message = ex.Message;
		}

		[DataMember]
		public string ErrorCode { get; set; }

		[DataMember]
		public string Message { get; set; }
	}
}
