using System;
using System.Globalization;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TeamMailboxSyncInfoCache : LazyLookupTimeoutCache<TeamMailboxSyncId, TeamMailboxSyncInfo>
	{
		public TeamMailboxSyncInfoCache(IResourceMonitorFactory resourceMonitorFactory, TimeSpan cacheSlidingExpiry, int cacheBucketCount, int cacheBucketSize, string syncLogConfigurationName) : base(cacheBucketCount, cacheBucketSize, true, cacheSlidingExpiry, TimeSpan.MaxValue)
		{
			this.resourceMonitorFactory = resourceMonitorFactory;
			this.syncLogConfigurationName = syncLogConfigurationName;
		}

		public static string LocalServerFqdn
		{
			get
			{
				if (TeamMailboxSyncInfoCache.localServerFqdn == null)
				{
					lock (TeamMailboxSyncInfoCache.syncObject)
					{
						if (TeamMailboxSyncInfoCache.localServerFqdn == null)
						{
							ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 85, "LocalServerFqdn", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\TeamMailboxSyncInfoCache.cs");
							TeamMailboxSyncInfoCache.localServerFqdn = topologyConfigurationSession.FindLocalServer().Fqdn;
						}
					}
				}
				return TeamMailboxSyncInfoCache.localServerFqdn;
			}
		}

		protected override TeamMailboxSyncInfo CreateOnCacheMiss(TeamMailboxSyncId key, ref bool shouldAdd)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(key.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key.OrgId), 105, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\LinkedFolder\\TeamMailboxSyncInfoCache.cs");
			ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindByExchangeGuid(key.MailboxGuid);
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				return null;
			}
			TeamMailboxLifecycleState teamMailboxLifecycleState = TeamMailboxLifecycleState.Active;
			if (TeamMailbox.IsPendingDeleteSiteMailbox(aduser))
			{
				teamMailboxLifecycleState = TeamMailboxLifecycleState.PendingDelete;
			}
			else if (aduser.SharePointUrl == null)
			{
				teamMailboxLifecycleState = TeamMailboxLifecycleState.Unlinked;
			}
			ExchangePrincipal exchangePrincipal = null;
			try
			{
				exchangePrincipal = ExchangePrincipal.FromMailboxGuid(ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key.OrgId), key.MailboxGuid, key.DomainController);
			}
			catch (ObjectNotFoundException)
			{
				return null;
			}
			TeamMailboxSyncInfo result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MailboxSession mailboxSession = null;
				UserConfiguration userConfiguration = null;
				if (teamMailboxLifecycleState == TeamMailboxLifecycleState.Active)
				{
					mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=TeamMailbox;Action=CommitChanges;Interactive=False");
					disposeGuard.Add<MailboxSession>(mailboxSession);
					if (!string.Equals(mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn, TeamMailboxSyncInfoCache.LocalServerFqdn, StringComparison.OrdinalIgnoreCase))
					{
						throw new WrongServerException(new LocalizedString(string.Format("Non-local XSO MailboxSession not allowed for TeamMailboxSync. TeamMailbox Name: {0}, MailboxGuid {1}, ServerFqdn {2}", mailboxSession.MailboxOwner.MailboxInfo.DisplayName, mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, mailboxSession.MailboxOwner.MailboxInfo.Location.ServerFqdn)));
					}
					userConfiguration = UserConfigurationHelper.GetMailboxConfiguration(mailboxSession, this.syncLogConfigurationName, UserConfigurationTypes.Stream, true);
					disposeGuard.Add<UserConfiguration>(userConfiguration);
				}
				TeamMailbox teamMailbox = TeamMailbox.FromDataObject(aduser);
				TeamMailboxSyncInfo teamMailboxSyncInfo = new TeamMailboxSyncInfo(key.MailboxGuid, teamMailboxLifecycleState, mailboxSession, exchangePrincipal, teamMailbox.DisplayName, teamMailbox.WebCollectionUrl, teamMailbox.WebId, (aduser.SharePointUrl != null) ? aduser.SharePointUrl.AbsoluteUri : null, (teamMailboxLifecycleState == TeamMailboxLifecycleState.Active) ? this.resourceMonitorFactory.Create(exchangePrincipal.MailboxInfo.MailboxDatabase.ObjectGuid) : null, userConfiguration);
				disposeGuard.Success();
				result = teamMailboxSyncInfo;
			}
			return result;
		}

		protected override bool HandleShouldRemove(TeamMailboxSyncId key, TeamMailboxSyncInfo value)
		{
			return value == null || !value.IsPending;
		}

		protected override void CleanupValue(TeamMailboxSyncId key, TeamMailboxSyncInfo value)
		{
			if (value != null)
			{
				if (value.Logger != null)
				{
					value.Logger.Dispose();
				}
				if (value.MailboxSession != null && !value.MailboxSession.IsDisposed)
				{
					value.MailboxSession.Dispose();
				}
			}
		}

		private static readonly object syncObject = new object();

		private readonly IResourceMonitorFactory resourceMonitorFactory;

		private readonly string syncLogConfigurationName;

		private static volatile string localServerFqdn;
	}
}
