using System;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal sealed class MsoCompanyMainStreamCookieManager : MsoMainStreamCookieManager
	{
		public MsoCompanyMainStreamCookieManager(string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval) : base(serviceInstanceName, maxCookieHistoryCount, cookieHistoryInterval)
		{
		}

		protected override MultiValuedProperty<byte[]> RetrievePersistedCookies(MsoMainStreamCookieContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			return container.MsoForwardSyncNonRecipientCookie;
		}

		protected override void LogPersistCookieEvent()
		{
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_CompanyMainStreamCookiePersisted, base.ServiceInstanceName, new object[]
			{
				base.ServiceInstanceName
			});
		}
	}
}
