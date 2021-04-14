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
using Microsoft.Exchange.PswsClient;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class ExCompliancePolicySyncNotificationClient : CompliancePolicySyncNotificationClient
	{
		private ExCompliancePolicySyncNotificationClient(Uri hostUrl, ICredentials credentials, Uri syncSvcUrl) : base(Workload.Exchange, credentials, syncSvcUrl)
		{
			ArgumentValidator.ThrowIfNull("hostUrl", hostUrl);
			this.pswsHostUrl = hostUrl;
		}

		public static CompliancePolicySyncNotificationClient Create(IConfigurationSession configurationSession, WriteVerboseDelegate writeVerboseDelegate)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			OrganizationId organizationId = configurationSession.GetOrgContainer().OrganizationId;
			return ProvisioningCache.Instance.TryAddAndGetOrganizationDictionaryValue<CompliancePolicySyncNotificationClient, Workload>(CannedProvisioningCacheKeys.OrganizationUnifiedPolicyNotificationClients, organizationId, Workload.Exchange, delegate()
			{
				if (writeVerboseDelegate != null)
				{
					writeVerboseDelegate(Strings.VerboseCreateNotificationClient(Workload.Exchange.ToString()));
				}
				Uri exoPswsHostUrlFromCache = CompliancePolicySyncNotificationClient.GetExoPswsHostUrlFromCache();
				Uri syncSvrUrlFromCache = CompliancePolicySyncNotificationClient.GetSyncSvrUrlFromCache(SyncSvcEndPointType.RestOAuth);
				ICredentials credentials = UnifiedPolicyConfiguration.GetInstance().GetCredentials(configurationSession, null);
				if (exoPswsHostUrlFromCache == null || syncSvrUrlFromCache == null)
				{
					throw new CompliancePolicySyncNotificationClientException(Strings.ErrorCannotInitializeNotificationClientToExchange(exoPswsHostUrlFromCache, syncSvrUrlFromCache));
				}
				ExCompliancePolicySyncNotificationClient result = new ExCompliancePolicySyncNotificationClient(exoPswsHostUrlFromCache, credentials, syncSvrUrlFromCache);
				if (writeVerboseDelegate != null)
				{
					writeVerboseDelegate(Strings.VerboseExNotificationClientInfo(exoPswsHostUrlFromCache, syncSvrUrlFromCache, credentials.GetType().Name));
				}
				return result;
			});
		}

		protected override string InternalNotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow)
		{
			NewCompliancePolicySyncNotificationCmdlet newCompliancePolicySyncNotificationCmdlet = new NewCompliancePolicySyncNotificationCmdlet();
			newCompliancePolicySyncNotificationCmdlet.HostServerName = this.pswsHostUrl.Host;
			newCompliancePolicySyncNotificationCmdlet.Port = this.pswsHostUrl.Port;
			newCompliancePolicySyncNotificationCmdlet.Authenticator = Authenticator.Create(this.credentials);
			newCompliancePolicySyncNotificationCmdlet.Identity = CompliancePolicySyncNotificationClient.GetClientInfoIdentifier(syncChangeInfos);
			newCompliancePolicySyncNotificationCmdlet.SyncSvcUrl = this.syncSvcUrl.AbsoluteUri;
			newCompliancePolicySyncNotificationCmdlet.FullSync = fullSync;
			newCompliancePolicySyncNotificationCmdlet.SyncNow = syncNow;
			newCompliancePolicySyncNotificationCmdlet.RequestTimeout = new int?(150000);
			if (syncChangeInfos != null)
			{
				newCompliancePolicySyncNotificationCmdlet.SyncChangeInfos = (from syncChangeInfo in syncChangeInfos
				select syncChangeInfo.ToString()).ToArray<string>();
			}
			CompliancePolicySyncNotification compliancePolicySyncNotification = newCompliancePolicySyncNotificationCmdlet.Run();
			if (compliancePolicySyncNotification == null || string.IsNullOrEmpty(compliancePolicySyncNotification.Identity))
			{
				throw new CompliancePolicySyncNotificationClientException(Strings.ErrorCompliancePolicySyncNotificationClient(Workload.Exchange.ToString(), (newCompliancePolicySyncNotificationCmdlet.Exception != null) ? newCompliancePolicySyncNotificationCmdlet.Exception.Message : string.Empty), newCompliancePolicySyncNotificationCmdlet.Exception);
			}
			return compliancePolicySyncNotification.Identity;
		}

		private readonly Uri pswsHostUrl;
	}
}
