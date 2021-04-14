using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoRecipientMainStreamCookieManager : MsoMainStreamCookieManager
	{
		public MsoRecipientMainStreamCookieManager(string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval) : base(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval)
		{
		}

		protected override MultiValuedProperty<byte[]> RetrievePersistedCookies(MsoMainStreamCookieContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			return container.MsoForwardSyncRecipientCookie;
		}

		protected override void LogPersistCookieEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_RecipientMainStreamCookiePersisted, base.ServiceInstanceName, new object[]
			{
				base.ServiceInstanceName
			});
		}
	}
}
