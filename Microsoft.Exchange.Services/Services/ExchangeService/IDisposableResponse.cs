using System;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal interface IDisposableResponse<TResponse> : IDisposable
	{
		TResponse Response { get; set; }
	}
}
