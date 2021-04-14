using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	[Serializable]
	internal class ExchangeServiceErrorResponseException : ExchangeServiceResponseException
	{
		public ExchangeServiceErrorResponseException(ResponseCodeType responseErrorCode, string responseErrorText) : base(CoreResources.ExchangeServiceResponseErrorWithCode(responseErrorCode.ToString(), responseErrorText))
		{
			this.ResponseErrorCode = responseErrorCode;
			this.ResponseErrorText = responseErrorText;
		}

		public ResponseCodeType ResponseErrorCode { get; private set; }

		public string ResponseErrorText { get; private set; }
	}
}
