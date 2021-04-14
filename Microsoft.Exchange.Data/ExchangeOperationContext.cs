using System;

namespace Microsoft.Exchange.Data
{
	internal class ExchangeOperationContext
	{
		public bool IsMoveUser { get; set; }

		public bool IsMoveSource { get; set; }

		public bool IsXForestMove { get; set; }

		public bool IsOlcMoveDestination { get; set; }
	}
}
