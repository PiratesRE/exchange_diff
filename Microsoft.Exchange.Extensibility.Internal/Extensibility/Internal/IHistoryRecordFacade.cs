using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal interface IHistoryRecordFacade
	{
		HistoryType Type { get; }

		RoutingAddress Address { get; }
	}
}
