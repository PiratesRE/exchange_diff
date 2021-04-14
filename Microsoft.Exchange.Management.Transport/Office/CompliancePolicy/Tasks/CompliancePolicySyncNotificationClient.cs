using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal abstract class CompliancePolicySyncNotificationClient
	{
		public Workload Workload { get; private set; }

		public CompliancePolicySyncNotificationClient(Workload workload, ICredentials credentials, Uri syncSvcUrl)
		{
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			ArgumentValidator.ThrowIfNull("syncSvcUrl", syncSvcUrl);
			this.Workload = workload;
			this.credentials = credentials;
			this.syncSvcUrl = syncSvcUrl;
		}

		public string NotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos)
		{
			return this.NotifyPolicyConfigChanges(syncChangeInfos, false, false);
		}

		public string NotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow)
		{
			if (!fullSync && (syncChangeInfos == null || !syncChangeInfos.Any<SyncChangeInfo>()))
			{
				throw new ArgumentException("syncChangeInfo must be provided if it is not full sync. In full sync scenario, please provide configuration type in syncChangeInfo; otherwise, this notification will trigger full sync for all supported types.");
			}
			string result;
			try
			{
				result = this.InternalNotifyPolicyConfigChanges(syncChangeInfos, fullSync, syncNow);
			}
			catch (Exception ex)
			{
				if (this.IsKnownException(ex))
				{
					throw new CompliancePolicySyncNotificationClientException(Strings.ErrorCompliancePolicySyncNotificationClient(this.Workload.ToString(), ex.Message), ex);
				}
				throw;
			}
			return result;
		}

		protected static string GetClientInfoIdentifier(IEnumerable<SyncChangeInfo> syncChangeInfos)
		{
			Guid guid = Guid.NewGuid();
			if (syncChangeInfos != null && syncChangeInfos.Any<SyncChangeInfo>())
			{
				PolicyVersion version = syncChangeInfos.First<SyncChangeInfo>().Version;
				if (version != null)
				{
					guid = version.InternalStorage;
				}
			}
			return string.Format("{0}:{1}", guid.ToString(), CompliancePolicySyncNotificationClient.clientInfo);
		}

		protected static Uri GetSyncSvrUrlFromCache(SyncSvcEndPointType endPointType = SyncSvcEndPointType.RestOAuth)
		{
			Uri uri = ProvisioningCache.Instance.TryAddAndGetGlobalDictionaryValue<Uri, string>(CannedProvisioningCacheKeys.GlobalUnifiedPolicyNotificationClientsInfo, "EopSyncSvcUrl", () => UnifiedPolicyConfiguration.GetInstance().GetSyncSvrBaseUrl());
			Uri result;
			switch (endPointType)
			{
			case SyncSvcEndPointType.RestOAuth:
				result = uri;
				break;
			case SyncSvcEndPointType.SoapOAuth:
				result = new Uri(uri.AbsoluteUri.TrimEnd(new char[]
				{
					'/'
				}) + "/soapoauth");
				break;
			case SyncSvcEndPointType.SoapCert:
				result = new Uri(uri.AbsoluteUri.TrimEnd(new char[]
				{
					'/'
				}) + "/soap");
				break;
			default:
				throw new NotSupportedException(endPointType + "is not supported by GetSyncSvrUrlFromCache");
			}
			return result;
		}

		protected static Uri GetExoPswsHostUrlFromCache()
		{
			return ProvisioningCache.Instance.TryAddAndGetGlobalDictionaryValue<Uri, string>(CannedProvisioningCacheKeys.GlobalUnifiedPolicyNotificationClientsInfo, "ExoPswsHostUrl", () => UnifiedPolicyConfiguration.GetInstance().GetExoPswsHostUrl());
		}

		protected abstract string InternalNotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow);

		protected virtual bool IsKnownException(Exception exception)
		{
			return exception is WebException;
		}

		private const string EopSyncSvcUrlCacheKey = "EopSyncSvcUrl";

		private const string ExoPswsHostUrlCacheKey = "ExoPswsHostUrl";

		private static readonly string clientInfo = string.Format("{0}_{1}", Environment.MachineName, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);

		protected readonly ICredentials credentials;

		protected readonly Uri syncSvcUrl;
	}
}
