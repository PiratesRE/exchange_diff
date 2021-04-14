using System;
using System.Web;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ArchiveServiceDiscoveryCache : BaseWebCache<string, string>
	{
		internal ArchiveServiceDiscoveryCache() : base("_XAU_", SlidingOrAbsoluteTimeout.Absolute, 10)
		{
		}

		public static ArchiveServiceDiscoveryCache Singleton
		{
			get
			{
				return ArchiveServiceDiscoveryCache.singleton;
			}
		}

		public void Remove(string key)
		{
			HttpRuntime.Cache.Remove(this.BuildKey(key));
		}

		private const string ArchiveServiceDiscoveryKeyPrefix = "_XAU_";

		private const int TimeoutInMinutes = 10;

		private static ArchiveServiceDiscoveryCache singleton = new ArchiveServiceDiscoveryCache();
	}
}
