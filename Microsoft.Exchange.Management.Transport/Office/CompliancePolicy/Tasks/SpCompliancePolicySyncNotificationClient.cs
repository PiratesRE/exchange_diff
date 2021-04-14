using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class SpCompliancePolicySyncNotificationClient : CompliancePolicySyncNotificationClient
	{
		private SpCompliancePolicySyncNotificationClient(Uri spSiteUrl, Uri spAdminSiteUrl, ICredentials credentials, Uri syncSvcUrl) : base(Workload.SharePoint, credentials, syncSvcUrl)
		{
			ArgumentValidator.ThrowIfNull("spSiteUrl", spSiteUrl);
			ArgumentValidator.ThrowIfNull("spAdminSiteUrl", spAdminSiteUrl);
			this.spPolicyCenterSite = new SpPolicyCenterSite(spSiteUrl, spAdminSiteUrl, credentials);
		}

		public static CompliancePolicySyncNotificationClient Create(IConfigurationSession configurationSession, WriteVerboseDelegate writeVerboseDelegate)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			OrganizationId organizationId = configurationSession.GetOrgContainer().OrganizationId;
			return ProvisioningCache.Instance.TryAddAndGetOrganizationDictionaryValue<CompliancePolicySyncNotificationClient, Workload>(CannedProvisioningCacheKeys.OrganizationUnifiedPolicyNotificationClients, organizationId, Workload.SharePoint, delegate()
			{
				if (writeVerboseDelegate != null)
				{
					writeVerboseDelegate(Strings.VerboseCreateNotificationClient(Workload.SharePoint.ToString()));
				}
				Uri syncSvrUrlFromCache = CompliancePolicySyncNotificationClient.GetSyncSvrUrlFromCache(SyncSvcEndPointType.RestOAuth);
				ICredentials credentials = UnifiedPolicyConfiguration.GetInstance().GetCredentials(configurationSession, null);
				Uri uri = null;
				Uri uri2 = null;
				UnifiedPolicyConfiguration.GetInstance().GetTenantSharePointUrls(configurationSession, out uri, out uri2);
				if (uri == null || uri2 == null || syncSvrUrlFromCache == null)
				{
					throw new CompliancePolicySyncNotificationClientException(Strings.ErrorCannotInitializeNotificationClientToSharePoint(uri, uri2, syncSvrUrlFromCache));
				}
				SpCompliancePolicySyncNotificationClient result = new SpCompliancePolicySyncNotificationClient(uri, uri2, credentials, syncSvrUrlFromCache);
				if (writeVerboseDelegate != null)
				{
					writeVerboseDelegate(Strings.VerboseSpNotificationClientInfo(uri, syncSvrUrlFromCache, credentials.GetType().Name));
				}
				return result;
			});
		}

		protected override string InternalNotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow)
		{
			string clientInfoIdentifier = CompliancePolicySyncNotificationClient.GetClientInfoIdentifier(syncChangeInfos);
			this.spPolicyCenterSite.NotifyUnifiedPolicySync(clientInfoIdentifier, this.syncSvcUrl.AbsoluteUri, (from syncChangeInfo in syncChangeInfos
			select syncChangeInfo.ToString()).ToArray<string>(), syncNow, fullSync);
			return clientInfoIdentifier;
		}

		protected override bool IsKnownException(Exception exception)
		{
			return exception is SpCsomCallException;
		}

		private SpPolicyCenterSite spPolicyCenterSite;
	}
}
