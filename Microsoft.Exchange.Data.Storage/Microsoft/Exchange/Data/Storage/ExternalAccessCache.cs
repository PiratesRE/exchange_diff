using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExternalAccessCache
	{
		private ExternalAccessCache()
		{
			this.refreshTimer = new GuardedTimer(delegate(object state)
			{
				this.ExpireEntriesInCache();
			});
			this.ExpireEntriesInCache();
		}

		public static ExternalAccessCache Instance
		{
			get
			{
				return ExternalAccessCache.instance;
			}
		}

		public bool IsExternalAccess(OrganizationId organizationId, SecurityIdentifier logonSid)
		{
			this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, organizationId.ToADSessionSettings(), 332, "IsExternalAccess", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Auditing\\AuditCaches.cs");
			Util.ThrowOnNullArgument(organizationId, "organizationId");
			Util.ThrowOnNullArgument(logonSid, "logonSid");
			Guid guid = Guid.Empty;
			bool flag = false;
			lock (this.externalAccessCache)
			{
				ExternalAccessCache.OrganizationInfo organizationInfo;
				if (this.externalAccessCache.TryGetValue(logonSid, out organizationInfo))
				{
					organizationInfo.LastAccessed = DateTime.UtcNow;
					guid = organizationInfo.OrgIdGuid;
					flag = true;
				}
			}
			if (!flag)
			{
				ExternalAccessCache.OrganizationInfo organizationInfo2 = this.GetOrganizationInfo(logonSid);
				if (organizationInfo2 == null)
				{
					return true;
				}
				lock (this.externalAccessCache)
				{
					if (!this.externalAccessCache.ContainsKey(logonSid))
					{
						this.externalAccessCache.Add(logonSid, organizationInfo2);
					}
				}
				guid = organizationInfo2.OrgIdGuid;
			}
			bool result;
			if (organizationId.OrganizationalUnit == null)
			{
				result = !(guid == Guid.Empty);
			}
			else
			{
				result = !organizationId.OrganizationalUnit.ObjectGuid.Equals(guid);
			}
			return result;
		}

		private ExternalAccessCache.OrganizationInfo GetOrganizationInfo(SecurityIdentifier logonSid)
		{
			Util.ThrowOnNullArgument(logonSid, "logonSid");
			ADRecipient recipient = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				recipient = this.recipientSession.FindBySid(logonSid);
			});
			if (adoperationResult.Succeeded && recipient != null)
			{
				return new ExternalAccessCache.OrganizationInfo
				{
					OrgIdGuid = ((recipient.OrganizationId.OrganizationalUnit == null) ? Guid.Empty : recipient.OrganizationId.OrganizationalUnit.ObjectGuid),
					LastAccessed = DateTime.UtcNow
				};
			}
			ProcessInfoEventLogger.Log(StorageEventLogConstants.Tuple_ErrorResolvingLogonUser, logonSid.ToString(), new object[]
			{
				logonSid,
				adoperationResult.Exception
			});
			return null;
		}

		private void ExpireEntriesInCache()
		{
			bool flag = false;
			try
			{
				Dictionary<SecurityIdentifier, ExternalAccessCache.OrganizationInfo> obj;
				Monitor.Enter(obj = this.externalAccessCache, ref flag);
				DateTime timeNow = DateTime.UtcNow;
				IEnumerable<SecurityIdentifier> source = from record in this.externalAccessCache.Keys
				where COWAudit.LastAuditAccessRefreshInterval < timeNow - this.externalAccessCache[record].LastAccessed
				select record;
				SecurityIdentifier[] array = source.ToArray<SecurityIdentifier>();
				foreach (SecurityIdentifier key in array)
				{
					this.externalAccessCache.Remove(key);
				}
			}
			finally
			{
				if (flag)
				{
					Dictionary<SecurityIdentifier, ExternalAccessCache.OrganizationInfo> obj;
					Monitor.Exit(obj);
				}
			}
			this.refreshTimer.Change(this.ExpiryInterval, new TimeSpan(0, 0, 0, 0, -1));
		}

		private readonly TimeSpan ExpiryInterval = new TimeSpan(6, 0, 0);

		private static readonly ExternalAccessCache instance = new ExternalAccessCache();

		private readonly Dictionary<SecurityIdentifier, ExternalAccessCache.OrganizationInfo> externalAccessCache = new Dictionary<SecurityIdentifier, ExternalAccessCache.OrganizationInfo>();

		private readonly GuardedTimer refreshTimer;

		private IRecipientSession recipientSession;

		private class OrganizationInfo
		{
			internal DateTime LastAccessed;

			internal Guid OrgIdGuid;
		}
	}
}
