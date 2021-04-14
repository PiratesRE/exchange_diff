using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class FullSyncPoller
	{
		public abstract IEnumerable<string> GetFullSyncTenants();
	}
}
