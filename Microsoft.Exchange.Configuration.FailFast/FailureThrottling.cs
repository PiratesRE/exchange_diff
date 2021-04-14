using System;
using System.Configuration;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal static class FailureThrottling
	{
		static FailureThrottling()
		{
			if (!int.TryParse(ConfigurationManager.AppSettings["FailureThrottlingLimit"], out FailureThrottling.failureThrottlingLimit))
			{
				FailureThrottling.failureThrottlingLimit = 15;
			}
			Logger.TraceDebug(ExTraceGlobals.FailureThrottlingTracer, "failureThrottlingLimit = {0}.", new object[]
			{
				FailureThrottling.failureThrottlingLimit
			});
		}

		internal static bool CountBasedOnStatusCode(string userToken, int statusCode)
		{
			int num;
			if (statusCode != 200)
			{
				if (statusCode != 400 && statusCode != 500)
				{
					return false;
				}
				num = 1;
			}
			else
			{
				num = -1;
			}
			FailureThrottling.FailureCounter counter = FailureThrottling.GetCounter(userToken);
			counter.AddDelta(num);
			Logger.TraceDebug(ExTraceGlobals.FailureThrottlingTracer, "User {0} counter changed to be {1}. Current status code is {2}.", new object[]
			{
				userToken,
				counter.Value,
				statusCode
			});
			if (num == 1 && counter.Value > FailureThrottling.failureThrottlingLimit)
			{
				Logger.TraceDebug(ExTraceGlobals.FailureThrottlingTracer, "User {0} is OverBudget. Counter: {1}. FailureThrottlingLimit: {2}", new object[]
				{
					userToken,
					counter.Value,
					FailureThrottling.failureThrottlingLimit
				});
				return true;
			}
			return false;
		}

		private static FailureThrottling.FailureCounter GetCounter(string userToken)
		{
			Logger.EnterFunction(ExTraceGlobals.FailureThrottlingTracer, "FailureThrottlingUserCache.GetCounter");
			FailureThrottling.FailureCounter failureCounter;
			FailureThrottling.failureThrottlingUserCache.TryGetValue(userToken, out failureCounter);
			if (failureCounter == null || !failureCounter.IsValid)
			{
				Logger.TraceDebug(ExTraceGlobals.FailureThrottlingTracer, "Create counter for User {0}.", new object[]
				{
					userToken
				});
				failureCounter = new FailureThrottling.FailureCounter();
				FailureThrottling.failureThrottlingUserCache.InsertAbsolute(userToken, failureCounter, FailureThrottling.FailureThrottlingTimePeriod, new RemoveItemDelegate<string, FailureThrottling.FailureCounter>(FailureThrottling.OnUserRemovedFromCache));
				FailFastModule.RemotePowershellPerfCounter.FailureThrottlingUserCacheSize.RawValue = (long)FailureThrottling.failureThrottlingUserCache.Count;
			}
			Logger.ExitFunction(ExTraceGlobals.FailureThrottlingTracer, "FailureThrottlingUserCache.GetCounter");
			return failureCounter;
		}

		private static void OnUserRemovedFromCache(string userToken, FailureThrottling.FailureCounter value, RemoveReason reason)
		{
			Logger.TraceDebug(ExTraceGlobals.FailureThrottlingTracer, "User {0} is removed from failure throttling user cache. Reason: {1}", new object[]
			{
				userToken,
				reason
			});
			FailFastModule.RemotePowershellPerfCounter.FailureThrottlingUserCacheSize.RawValue = (long)FailureThrottling.failureThrottlingUserCache.Count;
		}

		private static readonly TimeoutCache<string, FailureThrottling.FailureCounter> failureThrottlingUserCache = new TimeoutCache<string, FailureThrottling.FailureCounter>(20, 5000, false);

		private static readonly TimeSpan FailureThrottlingTimePeriod = TimeSpan.FromSeconds(10.0);

		private static readonly int failureThrottlingLimit;

		private class FailureCounter
		{
			internal FailureCounter()
			{
				this.addedTime = DateTime.UtcNow;
			}

			internal int Value
			{
				get
				{
					return this.value;
				}
			}

			internal bool IsValid
			{
				get
				{
					return this.addedTime + FailureThrottling.FailureThrottlingTimePeriod > DateTime.UtcNow;
				}
			}

			internal void AddDelta(int delta)
			{
				Interlocked.Add(ref this.value, delta);
			}

			private readonly DateTime addedTime;

			private int value;
		}
	}
}
