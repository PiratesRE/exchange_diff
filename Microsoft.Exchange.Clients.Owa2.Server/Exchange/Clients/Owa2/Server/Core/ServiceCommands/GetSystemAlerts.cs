using System;
using System.Configuration;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Online.BOX.Shell;
using Microsoft.Online.BOX.UI.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands
{
	internal class GetSystemAlerts : ServiceCommand<Alert[]>
	{
		public GetSystemAlerts(CallContext callContext) : base(callContext)
		{
			OwsLogRegistry.Register("GetSystemAlerts", typeof(GetSystemAlerts.GetSystemAlertsMetadata), new Type[0]);
		}

		protected override Alert[] InternalExecute()
		{
			string text = HttpContext.Current.Request.Headers["RPSOrgIdPUID"];
			string userPuid = string.IsNullOrEmpty(text) ? HttpContext.Current.Request.Headers["RPSPUID"] : text;
			string principalName = ((LiveIDIdentity)Thread.CurrentPrincipal.Identity).PrincipalName;
			string shellServiceUrl = string.Empty;
			string trackingGuid = Guid.NewGuid().ToString();
			Alert[] alerts;
			try
			{
				using (ShellServiceClient shellServiceClient = new ShellServiceClient("MsOnlineShellService_EndPointConfiguration"))
				{
					string certificateThumbprint = ConfigurationManager.AppSettings["MsOnlineShellService_CertThumbprint"];
					shellServiceClient.ClientCredentials.ClientCertificate.Certificate = TlsCertificateInfo.FindCertByThumbprint(certificateThumbprint);
					shellServiceUrl = shellServiceClient.Endpoint.Address.Uri.AbsoluteUri;
					GetAlertRequest getAlertRequest = new GetAlertRequest
					{
						WorkloadId = WorkloadAuthenticationId.Exchange,
						UserPuid = userPuid,
						UserPrincipalName = principalName,
						TrackingGuid = trackingGuid,
						CultureName = Thread.CurrentThread.CurrentUICulture.Name
					};
					alerts = shellServiceClient.GetAlerts(getAlertRequest);
				}
			}
			catch (Exception)
			{
				this.LogExceptionFromO365ShellService(principalName, userPuid, shellServiceUrl, trackingGuid);
				throw;
			}
			return alerts;
		}

		private void LogExceptionFromO365ShellService(string userPrincipalName, string userPuid, string shellServiceUrl, string trackingGuid)
		{
			SimulatedWebRequestContext.ExecuteWithoutUserContext("GetSystemAlerts", delegate(RequestDetailsLogger logger)
			{
				logger.ActivityScope.SetProperty(GetSystemAlerts.GetSystemAlertsMetadata.UserPrincipalName, userPrincipalName);
				logger.ActivityScope.SetProperty(GetSystemAlerts.GetSystemAlertsMetadata.UserPuid, userPuid);
				logger.ActivityScope.SetProperty(GetSystemAlerts.GetSystemAlertsMetadata.ShellServiceUrl, shellServiceUrl);
				logger.ActivityScope.SetProperty(GetSystemAlerts.GetSystemAlertsMetadata.TrackingGuid, trackingGuid);
			});
		}

		private const string EventId = "GetSystemAlerts";

		internal enum GetSystemAlertsMetadata
		{
			[DisplayName("GetSystemAlerts")]
			GetSystemAlertsId,
			[DisplayName("ShellServiceUrl")]
			ShellServiceUrl,
			[DisplayName("UserPuid")]
			UserPuid,
			[DisplayName("UserPrincipalName")]
			UserPrincipalName,
			[DisplayName("TrackingGuid")]
			TrackingGuid
		}
	}
}
