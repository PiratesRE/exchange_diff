using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class DeleteCalendarEventResponse : IExchangeWebMethodResponse
	{
		internal DeleteCalendarEventResponse(ServiceResult<VoidResult> serviceResult)
		{
			this.serviceResult = serviceResult;
		}

		ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return ResponseType.DeleteCalendarEventResponseMessage;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			ResponseCodeType result = ResponseCodeType.NoError;
			if (this.serviceResult.Code != ServiceResultCode.Success)
			{
				result = this.serviceResult.Error.MessageKey;
			}
			return result;
		}

		private readonly ServiceResult<VoidResult> serviceResult;
	}
}
