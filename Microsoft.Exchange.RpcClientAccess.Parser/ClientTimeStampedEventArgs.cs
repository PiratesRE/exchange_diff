using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class ClientTimeStampedEventArgs : ClientPerformanceEventArgs
	{
		public ExDateTime TimeStamp { get; private set; }

		public ClientTimeStampedEventArgs(uint blockTimeSinceRequest, ClientPerformanceEventType eventType) : base(eventType)
		{
			this.TimeStamp = ExDateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds(blockTimeSinceRequest));
		}
	}
}
