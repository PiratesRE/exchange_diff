using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.DataModel.Calendaring.CustomActions;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class ExpandCalendarEventResponse : IExchangeWebMethodResponse
	{
		internal ExpandCalendarEventResponse(ServiceResult<ExpandedEvent> serviceResult)
		{
			this.serviceResult = serviceResult;
		}

		public ExpandedEvent Result
		{
			get
			{
				return this.serviceResult.Value;
			}
		}

		ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return ResponseType.ExpandCalendarEventResponseMessage;
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

		private readonly ServiceResult<ExpandedEvent> serviceResult;
	}
}
