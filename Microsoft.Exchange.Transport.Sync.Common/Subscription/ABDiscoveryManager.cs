using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Transport.Sync.Common.Logging;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ABDiscoveryManager
	{
		public static IABSessionSettings GetSessionSettings(ExchangePrincipal exchangePrincipal, int? lcid, ConsistencyMode? consistencyMode, SyncLog syncLog, ClientSecurityContext clientSecurityContext)
		{
			return ABDiscoveryManager.GetSessionSettingsInternal(exchangePrincipal, null, lcid, consistencyMode, syncLog, clientSecurityContext);
		}

		public static IABSessionSettings GetSessionSettings(MailboxSession mailboxSession, int? lcid, ConsistencyMode? consistencyMode, SyncLog syncLog, ClientSecurityContext clientSecurityContext)
		{
			return ABDiscoveryManager.GetSessionSettingsInternal(mailboxSession.MailboxOwner, mailboxSession, lcid, consistencyMode, syncLog, clientSecurityContext);
		}

		private static IABSessionSettings GetSessionSettingsInternal(IExchangePrincipal exchangePrincipal, MailboxSession mailboxSession, int? lcid, ConsistencyMode? consistencyMode, SyncLog syncLog, ClientSecurityContext clientSecurityContext)
		{
			return ABDiscoveryManager.CreateSessionSettingsForAD(exchangePrincipal, mailboxSession, lcid, consistencyMode, clientSecurityContext);
		}

		private static IABSessionSettings CreateSessionSettingsForAD(IExchangePrincipal exchangePrincipal, MailboxSession mailboxSession, int? lcid, ConsistencyMode? consistencyMode, ClientSecurityContext clientSecurityContext)
		{
			int num;
			if (lcid != null)
			{
				num = lcid.Value;
			}
			else if (mailboxSession != null)
			{
				num = mailboxSession.Culture.LCID;
			}
			else
			{
				num = CultureInfo.CurrentCulture.LCID;
			}
			ConsistencyMode consistencyMode2;
			if (consistencyMode != null)
			{
				consistencyMode2 = consistencyMode.Value;
			}
			else
			{
				consistencyMode2 = ConsistencyMode.FullyConsistent;
			}
			ABSessionSettings absessionSettings = new ABSessionSettings();
			absessionSettings.Set("Provider", "AD");
			absessionSettings.Set("OrganizationId", exchangePrincipal.MailboxInfo.OrganizationId);
			absessionSettings.Set("Lcid", num);
			absessionSettings.Set("ConsistencyMode", consistencyMode2);
			absessionSettings.Set("ClientSecurityContext", clientSecurityContext);
			if (exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy != null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, exchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings(), 175, "CreateSessionSettingsForAD", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Common\\Subscription\\ABDiscoveryManager.cs");
				absessionSettings.Set("SearchRoot", DirectoryHelper.GetGlobalAddressListFromAddressBookPolicy(exchangePrincipal.MailboxInfo.Configuration.AddressBookPolicy, tenantOrTopologyConfigurationSession));
			}
			else
			{
				absessionSettings.Set("SearchRoot", null);
			}
			return absessionSettings;
		}
	}
}
