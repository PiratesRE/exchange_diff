using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Online.BOX.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class SuiteServiceProxyHelper
	{
		public string[] GetSuiteServiceProxyOriginAllowedList()
		{
			this.GetSuiteServiceInfo();
			string[] result = null;
			if (this.suiteServiceProxyInfo != null)
			{
				result = this.suiteServiceProxyInfo.SuiteServiceProxyOriginAllowedList;
			}
			return result;
		}

		public string GetSuiteServiceProxyScriptUrl()
		{
			this.GetSuiteServiceInfo();
			string result = string.Empty;
			if (this.suiteServiceProxyInfo != null)
			{
				result = this.suiteServiceProxyInfo.SuiteServiceProxyScriptUrl;
			}
			return result;
		}

		private void GetSuiteServiceInfo()
		{
			if (this.suiteServiceProxyInfo == null)
			{
				string text = HttpContext.Current.Request.Headers["RPSOrgIdPUID"];
				string userPuid = string.IsNullOrEmpty(text) ? HttpContext.Current.Request.Headers["RPSPUID"] : text;
				string principalName = ((LiveIDIdentity)Thread.CurrentPrincipal.Identity).PrincipalName;
				string shellServiceUrl = string.Empty;
				string trackingGuid = string.Empty;
				try
				{
					using (ShellServiceClient shellServiceClient = new ShellServiceClient("MsOnlineShellService_EndPointConfiguration"))
					{
						string certificateThumbprint = ConfigurationManager.AppSettings["MsOnlineShellService_CertThumbprint"];
						shellServiceClient.ClientCredentials.ClientCertificate.Certificate = TlsCertificateInfo.FindCertByThumbprint(certificateThumbprint);
						shellServiceUrl = shellServiceClient.Endpoint.Address.Uri.AbsoluteUri;
						trackingGuid = Guid.NewGuid().ToString();
						GetSuiteServiceInfoRequest getSuiteServiceInfoRequest = new GetSuiteServiceInfoRequest
						{
							WorkloadId = WorkloadAuthenticationId.Exchange,
							CultureName = CultureInfo.CurrentUICulture.Name,
							UserPuid = userPuid,
							UserPrincipalName = principalName,
							TrackingGuid = trackingGuid,
							UrlOfRequestingPage = HttpContext.Current.Request.QueryString["returnUrl"]
						};
						this.suiteServiceProxyInfo = shellServiceClient.GetSuiteServiceInfo(getSuiteServiceInfoRequest);
					}
				}
				catch (Exception exception)
				{
					this.suiteServiceProxyInfo = null;
					this.LogExceptionFromO365ShellService(exception, principalName, userPuid, shellServiceUrl);
				}
			}
		}

		private void LogExceptionFromO365ShellService(Exception exception, string userPrincipalName, string userPuid, string shellServiceUrl)
		{
			SimulatedWebRequestContext.ExecuteWithoutUserContext("GetShellInfo", delegate(RequestDetailsLogger logger)
			{
				OwsLogRegistry.Register("GetShellInfo", typeof(SuiteServiceProxyHelper.GetShellInfoMetadata), new Type[0]);
				logger.ActivityScope.SetProperty(ExtensibleLoggerMetadata.EventId, "GetShellInfo");
				logger.ActivityScope.SetProperty(OwaServerLogger.LoggerData.PrimarySmtpAddress, userPrincipalName);
				logger.ActivityScope.SetProperty(SuiteServiceProxyHelper.GetShellInfoMetadata.UserPuid, userPuid);
				logger.ActivityScope.SetProperty(SuiteServiceProxyHelper.GetShellInfoMetadata.ShellServiceUrl, shellServiceUrl);
				logger.ActivityScope.SetProperty(ServiceCommonMetadata.GenericErrors, exception.ToString());
			});
		}

		private SuiteServiceInfo suiteServiceProxyInfo;

		internal enum GetShellInfoMetadata
		{
			[DisplayName("GetShellInfo")]
			GetShellInfoId,
			[DisplayName("ShellServiceUrl")]
			ShellServiceUrl,
			[DisplayName("UserPuid")]
			UserPuid
		}
	}
}
