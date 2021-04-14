using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class OptionsResponseBase : BaseJsonResponse
	{
		public OptionsResponseBase(OptionsActionError errorCode)
		{
			this.WasSuccessful = false;
			this.ErrorCode = errorCode;
		}

		public OptionsResponseBase()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		[DataMember]
		public OptionsActionError ErrorCode { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public string UserPrompt { get; set; }
	}
}
