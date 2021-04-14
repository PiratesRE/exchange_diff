using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "AutodiscoverResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class AutodiscoverResponse
	{
		public AutodiscoverResponse()
		{
			this.errorCode = ErrorCode.NoError;
			this.errorMessage = string.Empty;
		}

		[DataMember(Name = "ErrorCode", Order = 1)]
		public ErrorCode ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.Set(ServiceCommonMetadata.ErrorCode, this.errorCode);
			}
		}

		[DataMember(Name = "ErrorMessage", Order = 2)]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.errorMessage = value;
				RequestDetailsLoggerBase<RequestDetailsLogger>.Current.AppendGenericError("ErrorMessage", this.errorMessage);
			}
		}

		private ErrorCode errorCode;

		private string errorMessage;
	}
}
