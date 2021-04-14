using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoCompanyFullSyncCookieManager : MsoFullSyncCookieManager
	{
		public MsoCompanyFullSyncCookieManager(Guid contextId) : base(contextId)
		{
		}

		protected override MultiValuedProperty<byte[]> RetrievePersistedPageTokens(MsoTenantCookieContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			return container.MsoForwardSyncNonRecipientCookie;
		}

		protected override void LogPersistPageTokenEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_TenantFullSyncCompanyPageTokenPersisted, base.ContextId.ToString(), new object[]
			{
				base.ContextId.ToString()
			});
		}

		protected override void LogClearPageTokenEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_TenantFullSyncCompanyPageTokenCleared, base.ContextId.ToString(), new object[]
			{
				base.ContextId.ToString()
			});
		}
	}
}
