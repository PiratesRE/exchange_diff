using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Hygiene.Deployment.Common;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class BasicRestOAuthSyncProxy : IPolicySyncWebserviceClient, IDisposable
	{
		public BasicRestOAuthSyncProxy(IHygieneLogger logger, string wsUrlBase, string acsUrl, X509Certificate2 acsTenantCertificate, string acsResourcePartnerId)
		{
			this.logger = logger;
			this.wsUrlBase = wsUrlBase;
			this.acsUrl = acsUrl;
			this.acsTenantCertificate = acsTenantCertificate;
			this.acsResourcePartnerId = acsResourcePartnerId;
			this.logger.LogMessage("FAM.BasicRestOAuthSyncProxy.ctor()");
			this.logger.LogVerbose(string.Format("wsUrlBase: '{0}'", wsUrlBase));
			this.logger.LogVerbose(string.Format("acsUrl: '{0}'", acsUrl));
			this.logger.LogVerbose(string.Format("acsTenantCertificate.Subject: '{0}'", (acsTenantCertificate != null) ? acsTenantCertificate.Subject : "(null)"));
			this.logger.LogVerbose(string.Format("acsResourcePartnerId: '{0}'", acsResourcePartnerId));
		}

		public GetChangesRequest InvalidGetChangesRequest { get; set; }

		public NetHelpersWebResponse MostRecentWebResponse { get; set; }

		public static BasicRestOAuthSyncProxy Create(IHygieneLogger logger, string wsUrlBase, string acsUrl, X509Certificate2 acsTenantCertificate, string acsResourcePartnerId)
		{
			return new BasicRestOAuthSyncProxy(logger, wsUrlBase, acsUrl, acsTenantCertificate, acsResourcePartnerId);
		}

		public static string GetTenantIdFromRequest(GetChangesRequest request)
		{
			string result = null;
			foreach (TenantCookie tenantCookie in ((IEnumerable<TenantCookie>)request.TenantCookies))
			{
				result = tenantCookie.TenantId.ToString();
			}
			return result;
		}

		public static string GetTenantIdFromRequest(PublishStatusRequest request)
		{
			string result = null;
			foreach (UnifiedPolicyStatus unifiedPolicyStatus in request.ConfigurationStatuses)
			{
				result = unifiedPolicyStatus.TenantId.ToString();
			}
			return result;
		}

		public static IEnumerable<Type> GetJsonKnownTypes()
		{
			return new Type[]
			{
				typeof(PolicyChangeBatch),
				typeof(PolicyConfiguration),
				typeof(RuleConfiguration),
				typeof(AssociationConfiguration),
				typeof(BindingConfiguration),
				typeof(IEnumerable<PolicyChangeBatch>),
				typeof(IEnumerable<PolicyConfiguration>),
				typeof(SyncCallerContext),
				typeof(TenantCookieCollection),
				typeof(TenantCookie)
			};
		}

		public void Dispose()
		{
		}

		public PolicyChangeBatch GetChanges(GetChangesRequest request)
		{
			string requestBody = JsonHelpers.SerializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeof(GetChangesRequest), request);
			if (this.InvalidGetChangesRequest != null)
			{
				requestBody = JsonHelpers.SerializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeof(GetChangesRequest), this.InvalidGetChangesRequest);
			}
			NetHelpersWebResponse netHelpersWebResponse = this.DoPolicySyncRequest("/GetChanges", BasicRestOAuthSyncProxy.GetTenantIdFromRequest(request), requestBody);
			this.MostRecentWebResponse = netHelpersWebResponse;
			if (netHelpersWebResponse == null)
			{
				return null;
			}
			this.logger.LogVerbose(string.Format("netResponse.Content: '{0}'", netHelpersWebResponse.Content));
			PolicyChangeBatch result;
			try
			{
				PolicyChangeBatch policyChangeBatch = (PolicyChangeBatch)JsonHelpers.DeserializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeof(PolicyChangeBatch), netHelpersWebResponse.Content);
				result = policyChangeBatch;
			}
			catch (SerializationException ex)
			{
				this.logger.LogError(string.Format("Received exception: '{0}'", ex.Message));
				this.logger.LogError(string.Format("netResponse.Content: '{0}'", netHelpersWebResponse.Content));
				result = null;
			}
			return result;
		}

		public void PublishStatus(PublishStatusRequest request)
		{
			string requestBody = JsonHelpers.SerializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeof(PublishStatusRequest), request);
			NetHelpersWebResponse mostRecentWebResponse = this.DoPolicySyncRequest("/PublishStatus", BasicRestOAuthSyncProxy.GetTenantIdFromRequest(request), requestBody);
			this.MostRecentWebResponse = mostRecentWebResponse;
		}

		public PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects)
		{
			string text = JsonHelpers.SerializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeof(SyncCallerContext), callerContext);
			text = "{\"callerContext\":{" + text + "}";
			string urlSuffix = string.Format("/GetObject?tenantId={0}&objectType={1}&objectId={2}&includeDeletedObjects={3}", new object[]
			{
				tenantId,
				objectType,
				objectId,
				includeDeletedObjects
			});
			NetHelpersWebResponse netHelpersWebResponse = this.DoPolicySyncRequest(urlSuffix, tenantId.ToString(), text);
			this.MostRecentWebResponse = netHelpersWebResponse;
			if (netHelpersWebResponse == null)
			{
				return null;
			}
			Type typeFromHandle;
			switch (objectType)
			{
			case ConfigurationObjectType.Policy:
				typeFromHandle = typeof(PolicyConfiguration);
				break;
			case ConfigurationObjectType.Rule:
				typeFromHandle = typeof(RuleConfiguration);
				break;
			case ConfigurationObjectType.Association:
				typeFromHandle = typeof(AssociationConfiguration);
				break;
			case ConfigurationObjectType.Binding:
				typeFromHandle = typeof(BindingConfiguration);
				break;
			case ConfigurationObjectType.Scope:
				typeFromHandle = typeof(ScopeConfiguration);
				break;
			default:
				throw new NotImplementedException("Ahhh!");
			}
			string text2 = netHelpersWebResponse.Content.Replace("{\"GetObjectResult\":", string.Empty);
			text2 = text2.Substring(0, text2.Length - 1);
			this.logger.LogVerbose(string.Format("Trimmed response: '{0}'", text2));
			PolicyConfigurationBase result;
			try
			{
				PolicyConfigurationBase policyConfigurationBase = (PolicyConfigurationBase)JsonHelpers.DeserializeJson(BasicRestOAuthSyncProxy.GetJsonKnownTypes(), typeFromHandle, text2);
				result = policyConfigurationBase;
			}
			catch (SerializationException ex)
			{
				this.logger.LogError(string.Format("Received exception: '{0}'", ex.Message));
				this.logger.LogError(string.Format("netResponse.Content: '{0}'", netHelpersWebResponse.Content));
				result = null;
			}
			return result;
		}

		public IAsyncResult BeginGetChanges(GetChangesRequest request, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public PolicyChangeBatch EndGetChanges(IAsyncResult asyncResult)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public PolicyChange GetSingleTenantChanges(TenantCookie tenantCookie)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public IAsyncResult BeginGetSingleTenantChanges(TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public PolicyChange EndGetSingleTenantChanges(IAsyncResult asyncResult)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public IAsyncResult BeginGetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public PolicyConfigurationBase EndGetObject(IAsyncResult asyncResult)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public IAsyncResult BeginPublishStatus(PublishStatusRequest request, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException("Not implemented.");
		}

		public void EndPublishStatus(IAsyncResult asyncResult)
		{
			throw new NotImplementedException("Not implemented.");
		}

		private NetHelpersWebResponse DoPolicySyncRequest(string urlSuffix, string tenantId, string requestBody)
		{
			OAuthTokenRequest oauthTokenRequest = OAuthHelpers.RequestTokenFromAcs(this.logger, this.acsUrl, this.acsTenantCertificate, new Uri(this.wsUrlBase).Host, this.acsResourcePartnerId, tenantId);
			if (string.IsNullOrEmpty(oauthTokenRequest.AcsTokenResultString))
			{
				this.logger.LogError("Null token from Acs");
				this.logger.LogError(string.Format("TenantId: '{0}'", tenantId));
				this.logger.LogError(string.Format("Audience: '{0}'", oauthTokenRequest.Audience));
				this.logger.LogError(string.Format("Issuer: '{0}'", oauthTokenRequest.Issuer));
				this.logger.LogError(string.Format("Resource: '{0}'", oauthTokenRequest.Resource));
				this.logger.LogError(string.Format("AcsUrl: '{0}'", oauthTokenRequest.AcsUrl));
				this.logger.LogError(string.Format("acsTenantCertificate.Subject: '{0}'", this.acsTenantCertificate.Subject));
				this.logger.LogError(string.Format("NetResponse.StatusCode: '{0}'", oauthTokenRequest.AcsNetResponse.WebResponse.StatusCode));
				this.logger.LogError(string.Format("jwtToken: '{0}'", oauthTokenRequest.JwtInputToken.ToString()));
				return null;
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(this.wsUrlBase + urlSuffix));
			httpWebRequest.Accept = "application/json";
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			httpWebRequest.Headers[HttpRequestHeader.Authorization] = string.Format("Bearer {0}", oauthTokenRequest.AcsTokenResultString);
			NetHelpersWebResponse netHelpersWebResponse = NetHelpers.DoWebRequest(this.logger, httpWebRequest, requestBody);
			if (netHelpersWebResponse.WebResponse.StatusCode != HttpStatusCode.OK)
			{
				this.logger.LogError("Status code from Policy Sync web request was not Ok");
				this.logger.LogError(string.Format("TenantId: '{0}'", tenantId));
				this.logger.LogError(string.Format("Audience: '{0}'", oauthTokenRequest.Audience));
				this.logger.LogError(string.Format("Issuer: '{0}'", oauthTokenRequest.Issuer));
				this.logger.LogError(string.Format("Resource: '{0}'", oauthTokenRequest.Resource));
				this.logger.LogError(string.Format("AcsUrl: '{0}'", oauthTokenRequest.AcsUrl));
				this.logger.LogError(string.Format("AcsTokenResultString: '{0}'", oauthTokenRequest.AcsTokenResultString));
				this.logger.LogError(string.Format("acsTenantCertificate.Subject: '{0}'", this.acsTenantCertificate.Subject));
				this.logger.LogError(string.Format("NetResponse.StatusCode: '{0}'", netHelpersWebResponse.WebResponse.StatusCode));
				this.logger.LogError(string.Format("jwtToken: '{0}'", oauthTokenRequest.JwtInputToken.ToString()));
			}
			return netHelpersWebResponse;
		}

		private IHygieneLogger logger = new NullHygieneLogger();

		private string wsUrlBase;

		private X509Certificate2 acsTenantCertificate;

		private string acsUrl;

		private string acsResourcePartnerId;
	}
}
