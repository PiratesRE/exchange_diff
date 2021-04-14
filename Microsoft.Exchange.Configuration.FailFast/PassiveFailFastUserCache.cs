using System;
using Microsoft.Exchange.Configuration.Core;
using Microsoft.Exchange.Diagnostics.Components.FailFast;

namespace Microsoft.Exchange.Configuration.FailFast
{
	internal class PassiveFailFastUserCache : FailFastUserCache
	{
		private PassiveFailFastUserCache() : base(1, 1)
		{
			this.passiveObjectBehavior = new CrossAppDomainPassiveObjectBehavior(FailFastUserCache.PipeNameOfThisProcess, BehaviorDirection.Out);
		}

		internal static PassiveFailFastUserCache Singleton
		{
			get
			{
				return PassiveFailFastUserCache.singleton;
			}
		}

		internal override bool IsUserInCache(string userToken, string userTenant, out string cacheKey, out FailFastUserCacheValue cacheValue, out BlockedReason blockedReason)
		{
			throw new NotSupportedException("The IsUserBlocked should not be invoked from PassiveFailFastUserCache.");
		}

		protected override void InsertValueToCache(string key, BlockedType blockedType, TimeSpan blockedTime)
		{
			if (!FailFastUserCache.FailFastEnabled)
			{
				return;
			}
			Logger.EnterFunction(ExTraceGlobals.FailFastCacheTracer, "PassiveFailFastUserCache.InsertValueToCache");
			long num = blockedTime.Ticks;
			if (num < 0L)
			{
				Logger.TraceDebug(ExTraceGlobals.FailFastCacheTracer, "Blocked time ticks {0} < 0", new object[]
				{
					num
				});
				num = 0L;
			}
			string text = string.Concat(new object[]
			{
				key,
				';',
				blockedType,
				';',
				num
			});
			byte[] bytes = FailFastUserCache.Encoding.GetBytes(text);
			Logger.TraceInformation(ExTraceGlobals.FailFastCacheTracer, "Send the blocked user info {0} to server stream.", new object[]
			{
				text
			});
			this.passiveObjectBehavior.SendMessage(bytes);
			Logger.ExitFunction(ExTraceGlobals.FailFastCacheTracer, "PassiveFailFastUserCache.InsertValueToCache");
		}

		private static PassiveFailFastUserCache singleton = new PassiveFailFastUserCache();

		private readonly CrossAppDomainPassiveObjectBehavior passiveObjectBehavior;
	}
}
