using System;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class FullSyncCookieManager : CookieManager
	{
		public MsoFullSyncCookie LastCookie { get; protected set; }

		protected FullSyncCookieManager(Guid contextId)
		{
			if (Guid.Empty.Equals(contextId))
			{
				throw new ArgumentException("contextId is Guid.Empty.");
			}
			this.ContextId = contextId;
		}

		public Guid ContextId { get; private set; }

		public abstract void ClearCookie();
	}
}
