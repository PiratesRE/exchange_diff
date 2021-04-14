using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class CreateCalendarEventResponse : IExchangeWebMethodResponse
	{
		internal CreateCalendarEventResponse(ServiceResult<Event>[] results)
		{
			this.aggregatedResponseCode = ResponseCodeType.NoError;
			this.Events = new Event[results.Length];
			for (int i = 0; i < results.Length; i++)
			{
				this.Events[i] = results[i].Value;
				if (results[i].Code != ServiceResultCode.Success)
				{
					this.aggregatedResponseCode = results[i].Error.MessageKey;
					return;
				}
			}
		}

		public Event[] Events { get; private set; }

		Microsoft.Exchange.Services.Core.Types.ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return Microsoft.Exchange.Services.Core.Types.ResponseType.CreateCalendarEventResponseMessage;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			return this.aggregatedResponseCode;
		}

		private readonly ResponseCodeType aggregatedResponseCode;
	}
}
