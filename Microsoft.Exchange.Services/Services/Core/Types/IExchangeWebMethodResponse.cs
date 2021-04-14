using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IExchangeWebMethodResponse
	{
		ResponseType GetResponseType();

		ResponseCodeType GetErrorCodeToLog();
	}
}
