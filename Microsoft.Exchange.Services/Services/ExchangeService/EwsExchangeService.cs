using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.ExchangeService
{
	internal class EwsExchangeService : ExchangeServiceBase
	{
		public EwsExchangeService(CallContext callContext)
		{
			ArgumentValidator.ThrowIfNull("callContext", callContext);
			base.CallContext = callContext;
		}

		protected override void InternalDispose(bool disposing)
		{
		}
	}
}
