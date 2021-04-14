using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal class UnifiedPolicyConfiguration
	{
		protected UnifiedPolicyConfiguration()
		{
			this.configurationFromFile = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
		}

		public static UnifiedPolicyConfiguration GetInstance()
		{
			if (UnifiedPolicyConfiguration.instance == null)
			{
				lock (UnifiedPolicyConfiguration.lockObject)
				{
					if (UnifiedPolicyConfiguration.instance == null)
					{
						if (ExPolicyConfigProvider.IsFFOOnline)
						{
							Assembly assembly = Assembly.Load("Microsoft.Exchange.Hygiene.ManagementHelper");
							Type type = assembly.GetType("Microsoft.Exchange.Hygiene.ManagementHelper.UnifiedPolicyConfigurationHelper");
							UnifiedPolicyConfiguration.instance = (UnifiedPolicyConfiguration)Activator.CreateInstance(type);
						}
						else
						{
							UnifiedPolicyConfiguration.instance = new UnifiedPolicyConfiguration();
						}
					}
				}
			}
			return UnifiedPolicyConfiguration.instance;
		}

		public static void ForceRecreationForTest()
		{
			lock (UnifiedPolicyConfiguration.lockObject)
			{
				UnifiedPolicyConfiguration.instance = null;
			}
		}

		public Uri GetExoPswsHostUrl()
		{
			Uri uri = this.GetUrlFromConfigFile("UP_PswsHostUrl");
			if (uri == null)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 160, "GetExoPswsHostUrl", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\UnifiedPolicyConfiguration.cs");
				ServiceEndpoint endpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint(ServiceEndpointId.ExchangeLoginUrl);
				uri = endpoint.Uri;
			}
			return uri;
		}

		public Uri GetSyncSvrBaseUrl()
		{
			Uri urlFromConfigFile = this.GetUrlFromConfigFile("UP_SyncSvcUrl");
			if (urlFromConfigFile == null)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 183, "GetSyncSvrBaseUrl", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\UnifiedPolicyConfiguration.cs");
				ServiceEndpoint endpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("FfoDataService");
				return new Uri(string.Format("{0}://{1}:{2}/PolicySync/PolicySync.svc", endpoint.Uri.Scheme, endpoint.Uri.Host, endpoint.Uri.Port));
			}
			return urlFromConfigFile;
		}

		public virtual void GetTenantSharePointUrls(IConfigurationSession configurationSession, out Uri spRootUrl, out Uri spAdminUrl)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			spRootUrl = null;
			spAdminUrl = null;
			string organizationIdKey = this.GetOrganizationIdKey(configurationSession);
			if (!string.IsNullOrEmpty(organizationIdKey))
			{
				spRootUrl = this.GetUrlFromConfigFile("UP_SPRootSite_" + organizationIdKey);
				spAdminUrl = this.GetUrlFromConfigFile("UP_SPAdminSite_" + organizationIdKey);
			}
		}

		public virtual Workload GetTenantSupportedWorkload(IConfigurationSession configurationSession)
		{
			Workload result = Workload.None;
			string organizationIdKey = this.GetOrganizationIdKey(configurationSession);
			string stringFromConfigFile = this.GetStringFromConfigFile("UP_TenantWorkloads_" + organizationIdKey);
			if (!string.IsNullOrEmpty(stringFromConfigFile))
			{
				Enum.TryParse<Workload>(stringFromConfigFile, out result);
			}
			return result;
		}

		public ICredentials GetCredentials(IConfigurationSession configurationSession, ADUser actAsUser = null)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			ICredentials credentials = null;
			string organizationIdKey = this.GetOrganizationIdKey(configurationSession);
			if (!string.IsNullOrEmpty(organizationIdKey))
			{
				string stringFromConfigFile = this.GetStringFromConfigFile("UP_TenantAdminCred_" + organizationIdKey);
				int num = stringFromConfigFile.IndexOf(':');
				if (num > 0)
				{
					credentials = new NetworkCredential(stringFromConfigFile.Substring(0, num), stringFromConfigFile.Substring(num + 1));
				}
			}
			if (credentials == null)
			{
				Organization orgContainer = configurationSession.GetOrgContainer();
				if (orgContainer != null && orgContainer.OrganizationId != null)
				{
					if (actAsUser != null)
					{
						credentials = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(orgContainer.OrganizationId, actAsUser, null);
					}
					else
					{
						credentials = OAuthCredentials.GetOAuthCredentialsForAppToken(orgContainer.OrganizationId, "PlaceHolder");
					}
				}
			}
			return credentials;
		}

		public virtual IEnumerable<string> GetUnifiedPolicyPreReqState(IConfigurationSession configurationSession)
		{
			throw new NotImplementedException();
		}

		public virtual void SetUnifiedPolicyPreReqState(IConfigurationSession configurationSession, IEnumerable<string> prerequisiteList)
		{
			throw new NotImplementedException();
		}

		public TimeSpan GetPolicyPendingStatusTimeout()
		{
			string stringFromConfigFile = this.GetStringFromConfigFile("UP_PendingStatusTimeoutInSeconds");
			int num;
			if (int.TryParse(stringFromConfigFile, out num) && num > 0)
			{
				return TimeSpan.FromSeconds((double)num);
			}
			return TimeSpan.FromHours(1.0);
		}

		public virtual string GetIntuneResourceUrl(IConfigurationSession configurationSession)
		{
			string stringFromConfigFile = this.GetStringFromConfigFile("IntuneResourceURL");
			if (!string.IsNullOrEmpty(stringFromConfigFile))
			{
				return stringFromConfigFile;
			}
			return null;
		}

		public virtual string GetIntuneEndpointUrl(IConfigurationSession configurationSession)
		{
			string stringFromConfigFile = this.GetStringFromConfigFile("IntuneEndPointURL");
			if (!string.IsNullOrEmpty(stringFromConfigFile))
			{
				return stringFromConfigFile;
			}
			return null;
		}

		public string GetOrganizationIdKey(IConfigurationSession configurationSession)
		{
			string result = string.Empty;
			Organization orgContainer = configurationSession.GetOrgContainer();
			if (orgContainer != null && orgContainer.OrganizationId != null)
			{
				result = orgContainer.OrganizationId.ToExternalDirectoryOrganizationId();
			}
			return result;
		}

		private Uri GetUrlFromConfigFile(string key)
		{
			Uri result = null;
			string stringFromConfigFile = this.GetStringFromConfigFile(key);
			if (!string.IsNullOrEmpty(stringFromConfigFile))
			{
				Uri.TryCreate(stringFromConfigFile, UriKind.Absolute, out result);
			}
			return result;
		}

		private string GetStringFromConfigFile(string key)
		{
			string result = string.Empty;
			KeyValueConfigurationElement keyValueConfigurationElement = this.configurationFromFile.AppSettings.Settings[key];
			if (keyValueConfigurationElement != null && !string.IsNullOrEmpty(keyValueConfigurationElement.Value))
			{
				result = keyValueConfigurationElement.Value;
			}
			return result;
		}

		private const string SyncSvcUrlKey = "UP_SyncSvcUrl";

		private const string ExoPswsHostUrlKey = "UP_PswsHostUrl";

		private const string PendingStatusTimeoutInSecondsKey = "UP_PendingStatusTimeoutInSeconds";

		private const string IntuneResourceURL = "IntuneResourceURL";

		private const string IntuneEndPointURL = "IntuneEndPointURL";

		private const string SPRootSiteKeyPrefix = "UP_SPRootSite_";

		private const string SPAdminSiteKeyPrefix = "UP_SPAdminSite_";

		private const string TenantAdminCredKeyPrefix = "UP_TenantAdminCred_";

		private const string TenantSupportedWorkloadKeyPrefix = "UP_TenantWorkloads_";

		private static UnifiedPolicyConfiguration instance;

		private static object lockObject = new object();

		private Configuration configurationFromFile;
	}
}
