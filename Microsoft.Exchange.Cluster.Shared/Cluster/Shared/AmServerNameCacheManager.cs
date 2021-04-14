using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class AmServerNameCacheManager : TimerComponent
	{
		public AmServerNameCacheManager() : base(new TimeSpan(0, 0, 0), TimeSpan.FromSeconds((double)RegistryParameters.AmServerNameCacheTTLInSec), "AmServerNameCacheManager")
		{
			AmServerNameCache.Instance.Enable();
		}

		protected override void TimerCallbackInternal()
		{
			AmServerNameCache.Instance.PopulateForDag();
		}
	}
}
