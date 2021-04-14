using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public abstract class ExternalProbe : ProbeWorkItem
	{
		protected abstract void PopulateEnterpriseAppSettings(out string tenantDomain, out Uri targetUri);

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				string userDomain;
				Uri uri;
				this.PopulateEnterpriseAppSettings(out userDomain, out uri);
				MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory = new MonitoringMailboxNotificationFactory();
				MailboxNotificationBatch notifications = monitoringMailboxNotificationFactory.CreateExternalMonitoringNotificationBatch();
				OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(OrganizationId.ForestWideOrgId, userDomain);
				OnPremPublisherServiceProxy onPremPublisherServiceProxy = this.CreateServiceProxy(uri, oauthCredentialsForAppToken);
				onPremPublisherServiceProxy.EndPublishOnPremNotifications(onPremPublisherServiceProxy.BeginPublishOnPremNotifications(notifications, null, null));
			}
			catch (Exception ex)
			{
				this.ReportFailure(ex);
			}
		}

		internal abstract OnPremPublisherServiceProxy CreateServiceProxy(Uri uri, ICredentials credentials);

		protected abstract void ReportFailure(Exception ex);
	}
}
