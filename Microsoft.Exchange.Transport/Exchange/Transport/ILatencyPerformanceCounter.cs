using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal interface ILatencyPerformanceCounter
	{
		LatencyPerformanceCounterType CounterType { get; }

		void AddValue(long latencySeconds);

		void AddValue(long latencySeconds, DeliveryPriority priority);

		void Update();

		void Reset();
	}
}
