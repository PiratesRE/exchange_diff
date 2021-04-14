using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal static class ExchangeServiceHelper
	{
		public static void CheckResponse(IExchangeWebMethodResponse response, ExecutionOption option)
		{
			if (response == null)
			{
				throw new ExchangeServiceResponseException(CoreResources.ExchangeServiceResponseErrorNoResponse);
			}
			ResponseMessage responseMessage = response as ResponseMessage;
			if (responseMessage != null && (option.ResponseValidationBehavior == ResponseValidationBehavior.ThrowOnSingleResponseError || option.ResponseValidationBehavior == ResponseValidationBehavior.ThrowOnAnyResponseError) && responseMessage.ResponseCode != ResponseCodeType.NoError)
			{
				throw new ExchangeServiceErrorResponseException(responseMessage.ResponseCode, responseMessage.MessageText);
			}
			BaseResponseMessage baseResponseMessage = response as BaseResponseMessage;
			if (baseResponseMessage != null)
			{
				if (baseResponseMessage.ResponseMessages.Items == null || baseResponseMessage.ResponseMessages.Items.Length == 0)
				{
					throw new ExchangeServiceResponseException(CoreResources.ExchangeServiceResponseErrorNoResponseForType(baseResponseMessage.ResponseType.ToString()));
				}
				if (option.ResponseValidationBehavior == ResponseValidationBehavior.ThrowOnAnyResponseError)
				{
					foreach (ResponseMessage responseMessage2 in baseResponseMessage.ResponseMessages.Items)
					{
						if (responseMessage2.ResponseCode != ResponseCodeType.NoError)
						{
							throw new ExchangeServiceErrorResponseException(responseMessage2.ResponseCode, responseMessage2.MessageText);
						}
					}
				}
			}
		}
	}
}
