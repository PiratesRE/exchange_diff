using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Extension;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class BposAssetReader<T> where T : BposNavBarInfo
	{
		internal BposAssetReader(string userPrincipalName, CultureInfo culture)
		{
			this.userPrincipalName = userPrincipalName;
			this.culture = culture;
			this.cacheKey = this.culture.Name;
			this.isMonitoringRequest = UserAgentUtilities.IsMonitoringRequest(HttpContext.Current.Request.UserAgent);
		}

		internal static void RegisterEvents(string eventId)
		{
			OwsLogRegistry.Register(eventId, typeof(BposAssetReader<T>.LogMetadata), new Type[0]);
		}

		public T GetData(AuthZClientInfo effectiveCaller)
		{
			if (this.data != null)
			{
				return this.data;
			}
			try
			{
				if (!this.isMonitoringRequest)
				{
					this.InvokeBposShellService(effectiveCaller);
				}
			}
			finally
			{
				if (this.data == null)
				{
					T cachedAssets = this.GetCachedAssets();
					if (cachedAssets != null)
					{
						this.data = cachedAssets;
					}
				}
			}
			return this.data;
		}

		private T GetCachedAssets()
		{
			T result = default(T);
			try
			{
				result = BposAssetCache<T>.Instance.Get(this.cacheKey);
			}
			catch (Exception)
			{
				result = default(T);
			}
			return result;
		}

		protected void UpdateCachedAssets(T bposNavBarInfo)
		{
			this.ClearWorkloadLinks(bposNavBarInfo.NavBarData);
			BposAssetCache<T>.Instance.Add(this.cacheKey, bposNavBarInfo);
		}

		private void InvokeBposShellService(AuthZClientInfo effectiveCaller)
		{
			string text = string.Empty;
			try
			{
				using (ShellServiceClient shellServiceClient = new ShellServiceClient("MsOnlineShellService_EndPointConfiguration"))
				{
					string certificateThumbprint = ConfigurationManager.AppSettings["MsOnlineShellService_CertThumbprint"];
					shellServiceClient.ClientCredentials.ClientCertificate.Certificate = TlsCertificateInfo.FindCertByThumbprint(certificateThumbprint);
					EndpointAddress address = shellServiceClient.Endpoint.Address;
					Uri uri = new Uri(address.Uri.AbsoluteUri);
					shellServiceClient.Endpoint.Address = new EndpointAddress(uri, address.Identity, new AddressHeader[0]);
					string text2 = HttpContext.Current.Request.Headers["RPSOrgIdPUID"];
					this.userPuid = (string.IsNullOrEmpty(text2) ? HttpContext.Current.Request.Headers["RPSPUID"] : text2);
					this.boxServiceUrl = shellServiceClient.Endpoint.Address.Uri.AbsoluteUri;
					text = Guid.NewGuid().ToString();
					OwaApplication.GetRequestDetailsLogger.ActivityScope.SetProperty(BposAssetReader<T>.LogMetadata.ShellRequestInfo, string.Format("OP:{0},UP:{1},UPN:{2},G:{3}", new object[]
					{
						text2,
						this.userPuid,
						this.userPrincipalName,
						text
					}));
					this.data = this.ExecuteRequest(shellServiceClient, this.culture.Name, this.userPrincipalName, this.userPuid, effectiveCaller, text);
					this.LogWorkloadLinks(this.data);
				}
			}
			catch (Exception e)
			{
				this.data = default(T);
				this.LogExceptionFromBposShellService(e, text);
			}
		}

		protected abstract T ExecuteRequest(ShellServiceClient client, string cultureName, string userPrincipalName, string userPuid, AuthZClientInfo effectiveCaller, string trackingGuid);

		protected NavBarData CreateNavBarData(string json)
		{
			MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(NavBarData));
			return (NavBarData)dataContractJsonSerializer.ReadObject(stream);
		}

		protected bool ShouldUpdateCache(string version)
		{
			T t = BposAssetCache<T>.Instance.Get(this.cacheKey);
			return t == null || t.Version != version;
		}

		protected void ClearWorkloadLinks(NavBarData navBarData)
		{
			if (navBarData != null)
			{
				navBarData.WorkloadLinks = null;
				navBarData.AdminLink = null;
				navBarData.PartnerLink = null;
				navBarData.UserDisplayName = null;
				navBarData.AppsLinks = null;
			}
		}

		protected void UpdateAppsLinks(NavBarData navBarData, AuthZClientInfo effectiveCaller)
		{
			string domain = string.Empty;
			if (navBarData.AppsLinks == null)
			{
				return;
			}
			if (effectiveCaller != null && !string.IsNullOrWhiteSpace(effectiveCaller.PrimarySmtpAddress))
			{
				SmtpAddress smtpAddress = new SmtpAddress(effectiveCaller.PrimarySmtpAddress);
				if (smtpAddress.IsValidAddress)
				{
					domain = smtpAddress.Domain;
				}
			}
			string deploymentId = ExtensionDataHelper.GetDeploymentId(domain);
			List<NavBarLinkData> list = new List<NavBarLinkData>(navBarData.AppsLinks.Length);
			NavBarLinkData[] appsLinks = navBarData.AppsLinks;
			int i = 0;
			while (i < appsLinks.Length)
			{
				NavBarLinkData navBarLinkData = appsLinks[i];
				if ("ShellMarketplace".Equals(navBarLinkData.Id, StringComparison.Ordinal))
				{
					if (Globals.IsPreCheckinApp)
					{
						navBarLinkData.Url = ExtensionData.GetClientExtensionMarketplaceUrl(this.culture.LCID, HttpContext.Current.Request, false, deploymentId, null);
						navBarLinkData.TargetWindow = "_blank";
						goto IL_169;
					}
					ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = ExchangeRunspaceConfigurationCache.Singleton.Get(effectiveCaller, null, false);
					if (exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MyMarketplaceApps) && (string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["OfficeStoreUnavailable"]) || StringComparer.OrdinalIgnoreCase.Equals("false", ConfigurationManager.AppSettings["OfficeStoreUnavailable"])))
					{
						navBarLinkData.Url = ExtensionData.GetClientExtensionMarketplaceUrl(this.culture.LCID, HttpContext.Current.Request, exchangeRunspaceConfiguration.HasRoleOfType(RoleType.MyReadWriteMailboxApps), deploymentId, null);
						navBarLinkData.TargetWindow = "_blank";
						goto IL_169;
					}
				}
				else
				{
					if ("ShellOfficeDotCom".Equals(navBarLinkData.Id, StringComparison.Ordinal))
					{
						navBarLinkData.TargetWindow = "_blank";
						goto IL_169;
					}
					goto IL_169;
				}
				IL_171:
				i++;
				continue;
				IL_169:
				list.Add(navBarLinkData);
				goto IL_171;
			}
			navBarData.AppsLinks = list.ToArray();
		}

		private void LogExceptionFromBposShellService(Exception e, string guid)
		{
			Exception baseException = e.GetBaseException();
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_BposHeaderConfigurationError, string.Empty, new object[]
			{
				baseException.ToString(),
				this.userPrincipalName,
				this.userPuid + " GUID: " + guid,
				this.boxServiceUrl
			});
		}

		private void LogWorkloadLinks(T data)
		{
			RequestDetailsLogger getRequestDetailsLogger = OwaApplication.GetRequestDetailsLogger;
			if (data != null && data.NavBarData != null && data.NavBarData.WorkloadLinks != null)
			{
				NavBarLinkData[] workloadLinks = data.NavBarData.WorkloadLinks;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (NavBarLinkData navBarLinkData in workloadLinks)
				{
					stringBuilder.AppendFormat("{0}:{1},", navBarLinkData.Id, navBarLinkData.Url);
				}
				getRequestDetailsLogger.ActivityScope.SetProperty(BposAssetReader<T>.LogMetadata.ShellResponseInfo, string.Format("L:{0}", stringBuilder.ToString()));
				return;
			}
			getRequestDetailsLogger.ActivityScope.SetProperty(BposAssetReader<T>.LogMetadata.ShellResponseInfo, string.Format("L:{0}", "NA"));
		}

		private const string ShellRequestInfoFormat = "OP:{0},UP:{1},UPN:{2},G:{3}";

		private const string ShellResponseInfoFormat = "L:{0}";

		private const string LinkFormat = "{0}:{1},";

		private const string NA = "NA";

		private readonly string userPrincipalName;

		private readonly CultureInfo culture;

		private readonly bool isMonitoringRequest;

		private readonly string cacheKey;

		private T data;

		private string userPuid;

		private string boxServiceUrl;

		private enum LogMetadata
		{
			[DisplayName("BPOS.SREQ.I")]
			ShellRequestInfo,
			[DisplayName("BPOS.SRES.I")]
			ShellResponseInfo
		}
	}
}
