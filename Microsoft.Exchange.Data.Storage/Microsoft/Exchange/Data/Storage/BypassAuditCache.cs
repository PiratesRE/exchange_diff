using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BypassAuditCache
	{
		private BypassAuditCache()
		{
		}

		public static BypassAuditCache Instance
		{
			get
			{
				return BypassAuditCache.instance;
			}
		}

		public bool IsUserBypassingAudit(OrganizationId organizationId, SecurityIdentifier logonSid)
		{
			Util.ThrowOnNullArgument(organizationId, "organizationId");
			Util.ThrowOnNullArgument(logonSid, "logonSid");
			bool flag = this.GetOrganizationCache(organizationId).IsUserBypassingAudit(logonSid);
			if (flag)
			{
				return true;
			}
			if (!organizationId.Equals(OrganizationId.ForestWideOrgId))
			{
				flag = this.GetOrganizationCache(OrganizationId.ForestWideOrgId).IsUserBypassingAudit(logonSid);
			}
			return flag;
		}

		public void Reset()
		{
			lock (this.organizationCaches)
			{
				this.organizationCaches.Clear();
			}
		}

		private BypassAuditCache.OrganizationCache GetOrganizationCache(OrganizationId organizationId)
		{
			BypassAuditCache.OrganizationCache result;
			lock (this.organizationCaches)
			{
				if (!this.organizationCaches.ContainsKey(organizationId))
				{
					this.organizationCaches[organizationId] = new BypassAuditCache.OrganizationCache(organizationId);
				}
				result = this.organizationCaches[organizationId];
			}
			return result;
		}

		private static readonly BypassAuditCache instance = new BypassAuditCache();

		private readonly Dictionary<OrganizationId, BypassAuditCache.OrganizationCache> organizationCaches = new Dictionary<OrganizationId, BypassAuditCache.OrganizationCache>();

		private class OrganizationCache
		{
			public OrganizationCache(OrganizationId organizationId)
			{
				Util.ThrowOnNullArgument(organizationId, "organizationId");
				this.adSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(BypassAuditCache.OrganizationCache.servicesRootOrgId, organizationId, null, false);
				this.refreshTimer = new GuardedTimer(delegate(object state)
				{
					this.RefreshCache();
				});
				this.RefreshCache();
			}

			internal bool IsUserBypassingAudit(SecurityIdentifier identity)
			{
				Util.ThrowOnNullArgument(identity, "identity");
				bool result = false;
				HashSet<SecurityIdentifier> hashSet = Interlocked.Exchange<HashSet<SecurityIdentifier>>(ref this.bypassingUserSid, this.bypassingUserSid);
				if (hashSet != null && hashSet.Contains(identity))
				{
					result = true;
				}
				return result;
			}

			private void RefreshCache()
			{
				try
				{
					IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.IgnoreInvalid, null, this.adSettings, ConfigScopes.TenantSubTree, 208, "RefreshCache", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Auditing\\AuditCaches.cs");
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AuditBypassEnabled, true);
					ADRawEntry[] recipients = null;
					ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
					{
						recipients = recipientSession.Find(null, QueryScope.SubTree, filter, null, 0, BypassAuditCache.OrganizationCache.queryProperties);
					});
					if (adoperationResult.Succeeded)
					{
						HashSet<SecurityIdentifier> hashSet = null;
						if (recipients != null && recipients.Length > 0)
						{
							hashSet = new HashSet<SecurityIdentifier>();
							foreach (ADRawEntry adrawEntry in recipients)
							{
								SecurityIdentifier securityIdentifier = adrawEntry[IADSecurityPrincipalSchema.Sid] as SecurityIdentifier;
								if (null != securityIdentifier && !hashSet.Contains(securityIdentifier))
								{
									hashSet.Add(securityIdentifier);
								}
							}
						}
						Interlocked.Exchange<HashSet<SecurityIdentifier>>(ref this.bypassingUserSid, hashSet);
					}
					else
					{
						ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorReadingBypassAudit, this.adSettings.CurrentOrganizationId.ToString(), new object[]
						{
							this.adSettings.CurrentOrganizationId,
							adoperationResult.Exception
						});
					}
				}
				finally
				{
					this.refreshTimer.Change(this.RefreshInterval, new TimeSpan(0, 0, 0, 0, -1));
				}
			}

			private readonly TimeSpan RefreshInterval = new TimeSpan(0, 30, 0);

			private static readonly PropertyDefinition[] queryProperties = new PropertyDefinition[]
			{
				IADSecurityPrincipalSchema.Sid
			};

			private static readonly ADObjectId servicesRootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();

			private readonly ADSessionSettings adSettings;

			private readonly GuardedTimer refreshTimer;

			private HashSet<SecurityIdentifier> bypassingUserSid;
		}
	}
}
