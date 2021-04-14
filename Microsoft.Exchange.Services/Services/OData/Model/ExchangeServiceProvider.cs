using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.ExchangeService;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal abstract class ExchangeServiceProvider
	{
		public ExchangeServiceProvider(IExchangeService exchangeService)
		{
			ArgumentValidator.ThrowIfNull("exchangeService", exchangeService);
			this.ExchangeService = exchangeService;
		}

		public IExchangeService ExchangeService { get; private set; }
	}
}
