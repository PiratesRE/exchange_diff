using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal enum ServerStatus
	{
		Searchable,
		NotFound,
		NotExchangeServer,
		LegacyExchangeServer
	}
}
