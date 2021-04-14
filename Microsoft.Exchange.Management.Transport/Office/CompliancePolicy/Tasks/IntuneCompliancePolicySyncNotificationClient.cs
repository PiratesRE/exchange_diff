using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.UnifiedPolicy;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.IdentityModel.S2S.Protocols.OAuth2;
using Microsoft.IdentityModel.S2S.Tokens;
using Microsoft.IdentityModel.SecurityTokenService;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal sealed class IntuneCompliancePolicySyncNotificationClient : CompliancePolicySyncNotificationClient
	{
		public IntuneCompliancePolicySyncNotificationClient(Workload workload, ICredentials credentials, Uri syncSvcUrl) : base(workload, credentials, syncSvcUrl)
		{
		}

		private static string CreateJsonNotificationBody(Guid tenantId, bool useFullSync, bool syncNow, IEnumerable<SyncChangeInfo> syncChangeInfos)
		{
			Uri syncSvrUrlFromCache = CompliancePolicySyncNotificationClient.GetSyncSvrUrlFromCache(SyncSvcEndPointType.SoapOAuth);
			IntuneCompliancePolicySyncNotificationClient.ODMSBody odmsbody = new IntuneCompliancePolicySyncNotificationClient.ODMSBody
			{
				Key = tenantId.ToString(),
				TenantId = tenantId.ToString(),
				SyncSvcUrl = syncSvrUrlFromCache.AbsoluteUri,
				FullSyncForTenant = useFullSync.ToString(),
				SyncNow = syncNow.ToString(),
				ChangeInfoList = new List<IntuneCompliancePolicySyncNotificationClient.ChangeInfo>()
			};
			foreach (SyncChangeInfo syncChangeInfo in syncChangeInfos)
			{
				odmsbody.ChangeInfoList.Add(new IntuneCompliancePolicySyncNotificationClient.ChangeInfo
				{
					ChangeType = syncChangeInfo.ChangeType.ToString("d"),
					ObjectId = syncChangeInfo.ObjectId.ToString(),
					ObjectType = syncChangeInfo.ObjectType.ToString("d"),
					Version = syncChangeInfo.Version.InternalStorage.ToString()
				});
			}
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(IntuneCompliancePolicySyncNotificationClient.ODMSBody));
				dataContractJsonSerializer.WriteObject(memoryStream, odmsbody);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				byte[] array = new byte[memoryStream.Length];
				result = ((memoryStream.Read(array, 0, (int)memoryStream.Length) > 0) ? Encoding.UTF8.GetString(array) : string.Empty);
			}
			return result;
		}

		private static string GetACSToken(OrganizationId tenantID, IConfigurationSession dataSession, ExecutionLog logger, Task task)
		{
			string result = null;
			LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(tenantID);
			LocalConfiguration configuration = ConfigProvider.Instance.Configuration;
			Uri uri = null;
			string text = null;
			string applicationId = configuration.ApplicationId;
			string text2 = null;
			foreach (PartnerApplication partnerApplication in configuration.PartnerApplications)
			{
				if (partnerApplication.Enabled && partnerApplication.Name.Contains("Intune"))
				{
					text2 = partnerApplication.ApplicationIdentifier;
					break;
				}
			}
			foreach (AuthServer authServer in configuration.AuthServers)
			{
				if (authServer.Enabled && authServer.Type == AuthServerType.MicrosoftACS)
				{
					text = authServer.IssuerIdentifier;
					uri = new Uri(authServer.TokenIssuingEndpoint);
					break;
				}
			}
			if (localTokenIssuer.SigningCert == null)
			{
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, "No certificate found.", null);
			}
			if (text2 == null)
			{
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, "No partnerId found.", null);
			}
			if (uri == null)
			{
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, "No authorizationEndpoint found.", null);
			}
			if (string.IsNullOrEmpty(text))
			{
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, "No issuerIdentifier found.", null);
			}
			if (localTokenIssuer.SigningCert != null && text2 != null && uri != null && !string.IsNullOrEmpty(text))
			{
				string arg = applicationId;
				string arg2 = text2;
				string intuneResourceUrl = UnifiedPolicyConfiguration.GetInstance().GetIntuneResourceUrl(dataSession);
				string arg3 = text;
				string authority = uri.Authority;
				string text3 = string.Format("{0}@{1}", arg, tenantID.ToExternalDirectoryOrganizationId());
				string text4 = string.Format("{0}/{1}@{2}", arg3, authority, tenantID.ToExternalDirectoryOrganizationId());
				string text5 = string.Format("{0}/{1}@{2}", arg2, intuneResourceUrl, tenantID.ToExternalDirectoryOrganizationId());
				X509SigningCredentials x509SigningCredentials = new X509SigningCredentials(localTokenIssuer.SigningCert, "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256", "http://www.w3.org/2001/04/xmlenc#sha256");
				JsonWebSecurityToken jsonWebSecurityToken = new JsonWebSecurityToken(text3, text4, DateTime.UtcNow, DateTime.UtcNow.AddMinutes(5.0), new List<JsonWebTokenClaim>(), x509SigningCredentials);
				OAuth2AccessTokenRequest oauth2AccessTokenRequest = OAuth2MessageFactory.CreateAccessTokenRequestWithAssertion(jsonWebSecurityToken, text5);
				OAuth2S2SClient oauth2S2SClient = new OAuth2S2SClient();
				try
				{
					OAuth2AccessTokenResponse oauth2AccessTokenResponse = (OAuth2AccessTokenResponse)oauth2S2SClient.Issue(uri.AbsoluteUri, oauth2AccessTokenRequest);
					if (oauth2AccessTokenResponse != null)
					{
						result = "Bearer " + oauth2AccessTokenResponse.AccessToken;
					}
				}
				catch (RequestFailedException ex)
				{
					ex.ToString();
					WebException ex2 = (WebException)ex.InnerException;
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex2.Response;
					Stream responseStream = httpWebResponse.GetResponseStream();
					Encoding encoding = Encoding.GetEncoding("utf-8");
					string text6 = "Auth service call failed: ";
					if (responseStream != null)
					{
						StreamReader streamReader = new StreamReader(responseStream, encoding);
						char[] array = new char[256];
						for (int k = streamReader.Read(array, 0, 256); k > 0; k = streamReader.Read(array, 0, 256))
						{
							text6 += new string(array, 0, k);
						}
					}
					logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, text6, ex);
				}
			}
			return result;
		}

		private static string Send(string url, string body, OrganizationId tenantid, IConfigurationSession dataSession, ExecutionLog logger, Task task)
		{
			HttpClientHandler handler = new HttpClientHandler
			{
				PreAuthenticate = true
			};
			string acstoken = IntuneCompliancePolicySyncNotificationClient.GetACSToken(tenantid, dataSession, logger, task);
			string result;
			using (HttpClient httpClient = new HttpClient(handler))
			{
				httpClient.DefaultRequestHeaders.Add("Authorization", acstoken);
				httpClient.DefaultRequestHeaders.Add("api-version", "1.0");
				StringContent content = new StringContent(body, Encoding.UTF8, "application/json");
				HttpResponseMessage httpResponseMessage = null;
				HttpWebResponse httpWebResponse = null;
				try
				{
					httpResponseMessage = httpClient.PutAsync(url, content).Result;
				}
				catch (WebException ex)
				{
					httpWebResponse = (HttpWebResponse)ex.Response;
					Stream responseStream = httpWebResponse.GetResponseStream();
					Encoding encoding = Encoding.GetEncoding("utf-8");
					string text = "Fail to notify: ";
					if (responseStream != null)
					{
						StreamReader streamReader = new StreamReader(responseStream, encoding);
						char[] array = new char[256];
						for (int i = streamReader.Read(array, 0, 256); i > 0; i = streamReader.Read(array, 0, 256))
						{
							text += new string(array, 0, i);
						}
					}
					logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Error, text, ex);
				}
				string text2;
				if (httpResponseMessage != null)
				{
					text2 = ((httpResponseMessage.StatusCode == HttpStatusCode.OK) ? string.Empty : (httpResponseMessage.StatusCode + " " + httpResponseMessage.Content.ReadAsStringAsync().Result));
				}
				else if (httpWebResponse != null)
				{
					text2 = httpWebResponse.StatusDescription;
				}
				else
				{
					text2 = "Failed";
				}
				result = text2;
			}
			return result;
		}

		private static string MakeNotificationCall(Task task, List<SyncChangeInfo> syncChangeInfos, bool useFullSync, bool syncNow, IConfigurationSession dataSession, ExecutionLog logger)
		{
			string text = dataSession.GetOrgContainer().OrganizationId.ToExternalDirectoryOrganizationId();
			Guid tenantId;
			if (!Guid.TryParse(text, out tenantId))
			{
				task.WriteWarning(Strings.WarningInvalidTenant(text));
				return "Error ExternalID not a guid";
			}
			string url = string.Format("{0}(guid'{1}')", UnifiedPolicyConfiguration.GetInstance().GetIntuneEndpointUrl(dataSession), text);
			string body = IntuneCompliancePolicySyncNotificationClient.CreateJsonNotificationBody(tenantId, useFullSync, syncNow, syncChangeInfos);
			string result = null;
			try
			{
				result = IntuneCompliancePolicySyncNotificationClient.Send(url, body, dataSession.GetOrgContainer().OrganizationId, dataSession, logger, task);
			}
			catch (WebException ex)
			{
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Warning, string.Format("We failed to notify workload '{0}'", Workload.Intune), ex);
				task.WriteVerbose(ex.ToString());
				result = ex.ToString();
			}
			return result;
		}

		private static ChangeNotificationData CreateChangeData(Workload workload, UnifiedPolicyStorageBase policyStorageObject)
		{
			Guid parentId = Guid.Empty;
			ConfigurationObjectType objectType = (policyStorageObject is ScopeStorage) ? ConfigurationObjectType.Scope : PolicyConfigConverterTable.GetConfigurationObjectType(policyStorageObject);
			if (policyStorageObject is RuleStorage)
			{
				parentId = ((RuleStorage)policyStorageObject).ParentPolicyId;
			}
			else if (policyStorageObject is BindingStorage)
			{
				parentId = ((BindingStorage)policyStorageObject).PolicyId;
			}
			return new ChangeNotificationData(policyStorageObject.Id.ObjectGuid, parentId, objectType, (policyStorageObject.ObjectState == ObjectState.Deleted) ? ChangeType.Delete : ChangeType.Update, workload, PolicyVersion.Create(policyStorageObject.PolicyVersion), UnifiedPolicyErrorCode.Unknown, "");
		}

		internal static IList<ChangeNotificationData> NotifyChange(Task task, UnifiedPolicyStorageBase policyStorageObject, IEnumerable<UnifiedPolicyStorageBase> relatedStorageObjects, IConfigurationSession dataSession, ExecutionLog logger)
		{
			Exception exception = null;
			string text = string.Empty;
			string empty = string.Empty;
			ChangeNotificationData changeNotificationData = IntuneCompliancePolicySyncNotificationClient.CreateChangeData(Workload.Intune, policyStorageObject);
			List<ChangeNotificationData> list = new List<ChangeNotificationData>
			{
				changeNotificationData
			};
			List<SyncChangeInfo> list2 = new List<SyncChangeInfo>();
			foreach (UnifiedPolicyStorageBase policyStorageObject2 in relatedStorageObjects)
			{
				list.Add(IntuneCompliancePolicySyncNotificationClient.CreateChangeData(Workload.Intune, policyStorageObject2));
			}
			foreach (ChangeNotificationData changeNotificationData2 in list)
			{
				SyncChangeInfo syncChangeInfo = changeNotificationData2.ShouldNotify ? changeNotificationData2.CreateSyncChangeInfo(true) : null;
				if (syncChangeInfo == null)
				{
					logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Warning, string.Format("We did not notify workload '{0}' for changes to objectId {1}", Workload.Intune, changeNotificationData2.Id), exception);
				}
				list2.Add(syncChangeInfo);
			}
			try
			{
				if (list2.Any<SyncChangeInfo>())
				{
					text = IntuneCompliancePolicySyncNotificationClient.MakeNotificationCall(task, list2, changeNotificationData.UseFullSync, changeNotificationData.ShouldNotify, dataSession, logger);
				}
			}
			catch (Exception ex)
			{
				text = Strings.ErrorMessageForNotificationFailure(Workload.Intune.ToString(), ex.Message);
				exception = ex;
			}
			if (!string.IsNullOrEmpty(text))
			{
				task.WriteWarning(Strings.WarningNotifyWorkloadFailed(changeNotificationData.ToString()));
				logger.LogOneEntry(task.GetType().Name, string.Empty, ExecutionLog.EventType.Warning, string.Format("We failed to notify workload '{0}' with error message '{1}'", Workload.Intune, text), exception);
				MonitoringItemErrorPublisher.Instance.PublishEvent("UnifiedPolicySync.SendNotificationError", UnifiedPolicyConfiguration.GetInstance().GetOrganizationIdKey(dataSession), string.Format("Workload={0};Timestamp={1}", Workload.Intune, DateTime.UtcNow), exception);
			}
			else
			{
				logger.LogOneEntry(ExecutionLog.EventType.Verbose, task.GetType().Name, empty, "Notification '{0}' was sent to workload '{1}' with sync change info: '{2}'", new object[]
				{
					empty,
					Workload.Intune,
					list2.First<SyncChangeInfo>().ToString()
				});
			}
			AggregatedNotificationClients.SetNotificationResults(list, text);
			return list;
		}

		protected override string InternalNotifyPolicyConfigChanges(IEnumerable<SyncChangeInfo> syncChangeInfos, bool fullSync, bool syncNow)
		{
			throw new NotImplementedException();
		}

		private const Workload workload = Workload.Intune;

		[DataContract]
		private class ODMSBody
		{
			[DataMember]
			public string Key;

			[DataMember]
			public string TenantId;

			[DataMember]
			public string SyncSvcUrl;

			[DataMember]
			public string FullSyncForTenant;

			[DataMember]
			public string SyncNow;

			[DataMember]
			public List<IntuneCompliancePolicySyncNotificationClient.ChangeInfo> ChangeInfoList;
		}

		[DataContract]
		private class ChangeInfo
		{
			[DataMember]
			public string ChangeType;

			[DataMember]
			public string ObjectId;

			[DataMember]
			public string ObjectType;

			[DataMember]
			public string Version;
		}
	}
}
