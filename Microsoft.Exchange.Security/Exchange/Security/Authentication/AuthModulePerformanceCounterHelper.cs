using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authentication.FederatedAuthService;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class AuthModulePerformanceCounterHelper
	{
		public static void Initialize(string protocol)
		{
			AuthModulePerformanceCounterHelper.counters = LiveIdBasicAuthenticationCounters.GetInstance(protocol);
			AuthModulePerformanceCounterHelper.percentageLogonCacheHitLastMinute = new SlidingPercentageCounter(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(1.0));
			AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute = new SlidingPercentageCounter(TimeSpan.FromMinutes(1.0), TimeSpan.FromSeconds(1.0));
			AuthModulePerformanceCounterHelper.percentileCountersTimer = new Timer(new TimerCallback(AuthModulePerformanceCounterHelper.UpdatePercentileCounters));
			AuthModulePerformanceCounterHelper.percentileCountersTimer.Change(60000, -1);
		}

		private static void UpdatePercentileCounters(object state)
		{
			AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute.GetSlidingPercentage();
			AuthModulePerformanceCounterHelper.counters.NumberOfRequestsLastMinute.RawValue = AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute.Denominator;
			if (AuthModulePerformanceCounterHelper.counters.NumberOfRequestsLastMinute.RawValue < 2L)
			{
				AuthModulePerformanceCounterHelper.counters.LogonCacheHit.RawValue = 0L;
				AuthModulePerformanceCounterHelper.counters.PercentageOfCookieBasedAuth.RawValue = 0L;
			}
			else
			{
				AuthModulePerformanceCounterHelper.counters.LogonCacheHit.RawValue = (long)((int)AuthModulePerformanceCounterHelper.percentageLogonCacheHitLastMinute.GetSlidingPercentage());
				AuthModulePerformanceCounterHelper.counters.PercentageOfCookieBasedAuth.RawValue = (long)((int)AuthModulePerformanceCounterHelper.percentageCookieHitLastMinute.GetSlidingPercentage());
			}
			AuthModulePerformanceCounterHelper.percentileCountersTimer.Dispose();
			AuthModulePerformanceCounterHelper.percentileCountersTimer = new Timer(new TimerCallback(AuthModulePerformanceCounterHelper.UpdatePercentileCounters));
			AuthModulePerformanceCounterHelper.percentileCountersTimer.Change(60000, -1);
		}

		private const int numberOfTotalRequestsToIgnoreInPercentileCounter = 2;

		private const int percentileCountersUpdateIntervalInSeconds = 60;

		private static Timer percentileCountersTimer = null;

		internal static SlidingPercentageCounter percentageLogonCacheHitLastMinute = null;

		internal static SlidingPercentageCounter percentageCookieHitLastMinute = null;

		internal static LiveIdBasicAuthenticationCountersInstance counters;
	}
}
