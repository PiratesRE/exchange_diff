using System;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Configuration.MonadDataProvider
{
	internal class MonadRunspaceCache : BasicRunspaceCache
	{
		internal MonadRunspaceCache()
		{
		}

		internal ExDateTime LastTimeCacheUsed
		{
			get
			{
				return this.lastCacheAccess;
			}
		}

		protected override bool AddRunspace(Runspace runspace)
		{
			this.lastCacheAccess = ExDateTime.UtcNow;
			return base.AddRunspace(runspace);
		}

		protected override Runspace RemoveRunspace()
		{
			this.lastCacheAccess = ExDateTime.UtcNow;
			return base.RemoveRunspace();
		}

		private ExDateTime lastCacheAccess;
	}
}
