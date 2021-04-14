using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Hygiene.Data.Directory;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class PolicySyncGetChangesAvailabilityProbe : ProbeWorkItem
	{
		public static string GetPolicySyncUrlHostName(IHygieneLogger logger, ProbeWorkItem probe)
		{
			ServiceEndpoint serviceEndpoint = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 60, "GetPolicySyncUrlHostName", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\PolicySyncGetChangesAvailabilityProbe.cs");
				serviceEndpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("FfoDataService");
			}
			catch (ServiceEndpointNotFoundException ex)
			{
				logger.LogError(string.Format("Exception while retrieving service endpoint: '{0}'", ex.Message));
				return null;
			}
			if (serviceEndpoint == null)
			{
				return null;
			}
			return serviceEndpoint.Uri.Host;
		}

		public static string GetPolicySyncCertificateSubject(IHygieneLogger logger, ProbeWorkItem probe)
		{
			ServiceEndpoint serviceEndpoint = null;
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 94, "GetPolicySyncCertificateSubject", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\PolicySyncGetChangesAvailabilityProbe.cs");
				serviceEndpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("FfoDataService");
			}
			catch (ServiceEndpointNotFoundException ex)
			{
				logger.LogError(string.Format("Exception while retrieving service endpoint: '{0}'", ex.Message));
				return null;
			}
			if (serviceEndpoint == null)
			{
				return null;
			}
			string certificateSubject = serviceEndpoint.CertificateSubject;
			logger.LogMessage(string.Format("certificateSubject='{0}'", certificateSubject));
			return certificateSubject;
		}

		public static Guid GetTenantIdForFeatureTag(IHygieneLogger logger, ProbeWorkItem probe)
		{
			string extensionAttribute = ProbeHelper.GetExtensionAttribute(logger, probe, "FeatureTag");
			if (string.IsNullOrEmpty(extensionAttribute))
			{
				return Guid.Empty;
			}
			GlobalConfigSession globalConfigSession = new GlobalConfigSession();
			IEnumerable<ProbeOrganizationInfo> probeOrganizations = globalConfigSession.GetProbeOrganizations(extensionAttribute);
			if (probeOrganizations.Count<ProbeOrganizationInfo>() == 0)
			{
				return Guid.Empty;
			}
			Guid objectGuid = probeOrganizations.ToArray<ProbeOrganizationInfo>()[PolicySyncGetChangesAvailabilityProbe.random.Next(probeOrganizations.Count<ProbeOrganizationInfo>())].ProbeOrganizationId.ObjectGuid;
			logger.LogMessage(string.Format("tenantGuid='{0}'", objectGuid));
			return objectGuid;
		}

		internal static IPolicySyncWebserviceClient CreateProxyClient(IHygieneLogger logger, ProbeWorkItem probe)
		{
			string extensionAttribute = ProbeHelper.GetExtensionAttribute(logger, probe, "SyncClientType");
			string extensionAttribute2 = ProbeHelper.GetExtensionAttribute(logger, probe, "CallerId");
			if (string.IsNullOrEmpty(extensionAttribute))
			{
				logger.LogError("syncClientType is null, exiting");
				return null;
			}
			if (string.IsNullOrEmpty(extensionAttribute2))
			{
				logger.LogError("callerId is null, exiting");
				return null;
			}
			if (string.Equals(extensionAttribute, "BasicSyncProxy", StringComparison.InvariantCultureIgnoreCase))
			{
				string policySyncUrl = "https://localhost/PolicySync/PolicySync.svc/soap";
				string policySyncCertificateSubject = PolicySyncGetChangesAvailabilityProbe.GetPolicySyncCertificateSubject(logger, probe);
				if (string.IsNullOrEmpty(policySyncCertificateSubject))
				{
					logger.LogError("certificateSubject is null, exiting");
					return null;
				}
				X509Certificate2Collection x509Certificate2Collection = TlsCertificateInfo.FindAllCertWithSubjectDistinguishedName(policySyncCertificateSubject, false);
				logger.LogMessage(string.Format("clientCertificateCollection.Count='{0}'", x509Certificate2Collection.Count));
				if (x509Certificate2Collection.Count == 0 || x509Certificate2Collection[0] == null)
				{
					logger.LogError(string.Format("No certificates found by subject '{0}', exiting", policySyncCertificateSubject));
					return null;
				}
				X509Certificate2 x509Certificate = x509Certificate2Collection[0];
				logger.LogMessage(string.Format("certificateSubject='{0}', clientCertificate.Subject='{1}'", policySyncCertificateSubject, x509Certificate.Subject));
				ProbeResult result = probe.Result;
				result.ExecutionContext += string.Format(string.Format("PROBE -- policySyncUrl '{0}', certificateSubject '{1}'. ", policySyncUrl, policySyncCertificateSubject), new object[0]);
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, new RemoteCertificateValidationCallback(delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
				{
					ProbeResult result3 = probe.Result;
					result3.ExecutionContext += string.Format(string.Format("PROBE -- callback Url '{0}' ", ((WebRequest)sender).RequestUri.ToString()), new object[0]);
					return ((WebRequest)sender).RequestUri.ToString().ToLower().Contains(policySyncUrl.ToLower());
				}));
				return BasicSyncProxy.Create(new EndpointAddress(new Uri(policySyncUrl), new AddressHeader[0]), x509Certificate, extensionAttribute2);
			}
			else
			{
				if (!string.Equals(extensionAttribute, "BasicRestOAuthSyncProxy", StringComparison.InvariantCultureIgnoreCase))
				{
					return null;
				}
				string policySyncUrlHostName = PolicySyncGetChangesAvailabilityProbe.GetPolicySyncUrlHostName(logger, probe);
				if (string.IsNullOrEmpty(policySyncUrlHostName))
				{
					logger.LogError("policySyncUrlHostName is null, exiting");
					return null;
				}
				string text = string.Format("https://{0}/PolicySync/PolicySync.svc", policySyncUrlHostName);
				X509Certificate2 x509Certificate2 = null;
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 222, "CreateProxyClient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ForefrontActiveMonitoring\\Components\\WebService\\Probes\\PolicySyncGetChangesAvailabilityProbe.cs");
				AuthConfig authConfig = AuthConfig.Read(topologyConfigurationSession);
				if (authConfig == null)
				{
					logger.LogError("AuthConfig is null, exiting");
					return null;
				}
				if (string.IsNullOrEmpty(authConfig.CurrentCertificateThumbprint))
				{
					logger.LogError("AuthConfig.CurrentCertificateThumbprint is null, exiting");
					return null;
				}
				try
				{
					x509Certificate2 = TlsCertificateInfo.FindCertByThumbprint(authConfig.CurrentCertificateThumbprint);
				}
				catch (ArgumentException ex)
				{
					logger.LogError(string.Format("Exception while retrieving cert: '{0}'", ex.Message));
					return null;
				}
				if (x509Certificate2 == null)
				{
					logger.LogError("acsCertificate is null, exiting");
					return null;
				}
				ServiceEndpoint serviceEndpoint = null;
				try
				{
					serviceEndpoint = topologyConfigurationSession.GetEndpointContainer().GetEndpoint("AcsService");
				}
				catch (ServiceEndpointNotFoundException ex2)
				{
					logger.LogError(string.Format("Exception while retrieving service endpoint: '{0}'", ex2.Message));
					return null;
				}
				if (serviceEndpoint == null)
				{
					logger.LogError(string.Format("acsService is null, exiting", new object[0]));
					return null;
				}
				ProbeResult result2 = probe.Result;
				result2.ExecutionContext += string.Format(string.Format("PROBE -- policySyncUrl '{0}', acsUrl '{1}', acsCertificateSubject '{2}'. ", text, serviceEndpoint.Uri.ToString(), x509Certificate2.Subject), new object[0]);
				return BasicRestOAuthSyncProxy.Create(logger, text, serviceEndpoint.Uri.ToString(), x509Certificate2, WellknownPartnerApplicationIdentifiers.ExchangeOnlineProtection);
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			this.logger = new ProbeWorkItemLogger(this, false, false);
			this.logger.LogMessage("PolicySyncProbe started");
			string extensionAttribute = ProbeHelper.GetExtensionAttribute(this.logger, this, "CallerId");
			Guid tenantIdForFeatureTag = PolicySyncGetChangesAvailabilityProbe.GetTenantIdForFeatureTag(this.logger, this);
			if (tenantIdForFeatureTag.Equals(Guid.Empty))
			{
				this.logger.LogError("Empty tenant guid, exiting");
				return;
			}
			IPolicySyncWebserviceClient policySyncWebserviceClient = PolicySyncGetChangesAvailabilityProbe.CreateProxyClient(this.logger, this);
			if (policySyncWebserviceClient == null)
			{
				this.logger.LogError("Null proxy, exiting");
				return;
			}
			this.logger.LogMessage("Proxy created successfully");
			List<Exception> list = new List<Exception>();
			foreach (ConfigurationObjectType objectType in new ConfigurationObjectType[]
			{
				ConfigurationObjectType.Policy,
				ConfigurationObjectType.Rule,
				ConfigurationObjectType.Association,
				ConfigurationObjectType.Binding
			})
			{
				Exception ex = this.CallGetChangesForType(policySyncWebserviceClient, extensionAttribute, tenantIdForFeatureTag, objectType);
				if (ex != null)
				{
					list.Add(ex);
					break;
				}
			}
			this.logger.LogMessage("Evaluating exceptions");
			if (list.Any<Exception>())
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (Exception ex2 in list)
				{
					stringBuilder.AppendLine(string.Format("{0}\n", ex2.Message));
				}
				throw new Exception(stringBuilder.ToString(), list.First<Exception>());
			}
			this.logger.LogMessage("No exceptions occurred");
			policySyncWebserviceClient.Dispose();
		}

		private Exception CallGetChangesForType(IPolicySyncWebserviceClient proxy, string callerId, Guid tenantGuid, ConfigurationObjectType objectType)
		{
			Exception result3;
			try
			{
				TenantCookieCollection tenantCookieCollection = new TenantCookieCollection(Workload.Exchange, objectType);
				tenantCookieCollection[tenantGuid] = new TenantCookie(tenantGuid, null, Workload.Exchange, objectType, null);
				PolicyChangeBatch changes = proxy.GetChanges(new GetChangesRequest
				{
					CallerContext = SyncCallerContext.Create(callerId),
					TenantCookies = tenantCookieCollection
				});
				if (proxy is BasicRestOAuthSyncProxy)
				{
					string arg = ((BasicRestOAuthSyncProxy)proxy).MostRecentWebResponse.WebResponse.Headers["HYGIENEWS-MACHINENAME"];
					string arg2 = ((BasicRestOAuthSyncProxy)proxy).MostRecentWebResponse.WebResponse.Headers["HYGIENEWS-VERSION"];
					ProbeResult result = base.Result;
					result.ExecutionContext += string.Format(string.Format("PROBE -- machineName {0} and version {1}. ", arg, arg2), new object[0]);
				}
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += string.Format(string.Format("PROBE -- GetChanges for objectType {0} and tenantId {1} returned {2}. ", objectType, tenantGuid, (changes == null) ? "a null batch" : string.Format("{0} changes", changes.Changes.Count<PolicyConfigurationBase>())), new object[0]);
				if (changes == null)
				{
					result3 = new Exception("Null batch.");
				}
				else
				{
					result3 = null;
				}
			}
			catch (Exception ex)
			{
				result3 = new Exception(string.Format("Received exception from PolicySync when calling GetChanges for object type {0} and tenantId {1}: {2}. ", objectType, tenantGuid, ex.Message), ex);
			}
			return result3;
		}

		private static Random random = new Random();

		private IHygieneLogger logger = new NullHygieneLogger();
	}
}
