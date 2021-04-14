using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public class GetCalendarViewResponse : IExchangeWebMethodResponse
	{
		internal GetCalendarViewResponse(ServiceResult<Event[]> results)
		{
			this.Events = results.Value;
			this.responseCode = ((results.Code == ServiceResultCode.Success) ? ResponseCodeType.NoError : results.Error.MessageKey);
		}

		public Event[] Events { get; private set; }

		Microsoft.Exchange.Services.Core.Types.ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return Microsoft.Exchange.Services.Core.Types.ResponseType.GetCalendarViewResponseMessage;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			return this.responseCode;
		}

		private readonly ResponseCodeType responseCode;
	}
}
