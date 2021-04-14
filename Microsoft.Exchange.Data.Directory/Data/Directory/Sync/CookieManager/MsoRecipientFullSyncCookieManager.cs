using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoRecipientFullSyncCookieManager : MsoFullSyncCookieManager
	{
		public MsoRecipientFullSyncCookieManager(Guid contextId) : base(contextId)
		{
		}

		protected override MultiValuedProperty<byte[]> RetrievePersistedPageTokens(MsoTenantCookieContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			return container.MsoForwardSyncRecipientCookie;
		}

		protected override void LogPersistPageTokenEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_TenantFullSyncRecipientPageTokenPersisted, base.ContextId.ToString(), new object[]
			{
				base.ContextId.ToString()
			});
		}

		protected override void LogClearPageTokenEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_TenantFullSyncRecipientPageTokenCleared, base.ContextId.ToString(), new object[]
			{
				base.ContextId.ToString()
			});
		}
	}
}
