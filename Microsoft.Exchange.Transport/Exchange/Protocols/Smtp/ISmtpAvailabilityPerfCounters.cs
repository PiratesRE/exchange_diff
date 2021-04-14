using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal interface ISmtpAvailabilityPerfCounters
	{
		void UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory category);

		void IncrementMessageLoopsInLastHourCounter(long incrementValue);
	}
}
