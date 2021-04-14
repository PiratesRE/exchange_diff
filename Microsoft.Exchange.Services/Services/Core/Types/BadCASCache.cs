using System;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class BadCASCache : BaseWebCache<string, string>
	{
		internal BadCASCache() : base("_BCC_", SlidingOrAbsoluteTimeout.Absolute, 5)
		{
		}

		protected override bool ValidateAddition(string key, string value)
		{
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[BadCASCache::ValidateAddition] A CAS is being added to the bad CAS cache.  FQDN: {0}", key);
			return base.ValidateAddition(key, value);
		}

		public static BadCASCache Singleton
		{
			get
			{
				return BadCASCache.singleton;
			}
		}

		private const string BadCASKeyPrefix = "_BCC_";

		private const int TimeoutInMinutes = 5;

		private static BadCASCache singleton = new BadCASCache();
	}
}
