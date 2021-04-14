using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MaskAutoCompleteRecipientResponse : IExchangeWebMethodResponse
	{
		public MaskAutoCompleteRecipientResponse()
		{
			this.WasSuccessful = true;
		}

		[DataMember]
		public bool WasSuccessful { get; set; }

		ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return ResponseType.MaskAutoCompleteRecipientResponseMessage;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			return ResponseCodeType.NoError;
		}
	}
}
