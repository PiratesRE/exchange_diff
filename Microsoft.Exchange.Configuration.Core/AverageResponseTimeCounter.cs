using System;

namespace Microsoft.Exchange.Configuration.Core
{
	public class AverageResponseTimeCounter
	{
		internal long GetAverageResponseTime(long requestTime, long lastValue)
		{
			try
			{
				this.accumulatedRequestSec += requestTime;
			}
			catch
			{
				this.accumulatedRequestSec = lastValue;
				this.accumulatedRequestCounts = 1;
				return 0L;
			}
			this.accumulatedRequestCounts++;
			return this.accumulatedRequestSec / (long)this.accumulatedRequestCounts;
		}

		private int accumulatedRequestCounts;

		private long accumulatedRequestSec;
	}
}
