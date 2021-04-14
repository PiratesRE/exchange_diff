using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public class EnterpriseConnectivityProbe : ExternalProbe
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
		}

		internal override OnPremPublisherServiceProxy CreateServiceProxy(Uri uri, ICredentials credentials)
		{
			return OnPremPublisherServiceProxy.CreateMonitoringProxy(uri, credentials, null);
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		protected override void PopulateEnterpriseAppSettings(out string tenantDomain, out Uri targetUri)
		{
			PushNotificationPublisherConfiguration pushNotificationPublisherConfiguration = new PushNotificationPublisherConfiguration(false, null);
			ProxyPublisherSettings proxyPublisherSettings = pushNotificationPublisherConfiguration.ProxyPublisherSettings;
			if (proxyPublisherSettings == null)
			{
				throw new Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.PushNotificationConfigurationException(Strings.PushNotificationEnterpriseNotConfigured);
			}
			if (string.IsNullOrEmpty(proxyPublisherSettings.ChannelSettings.Organization))
			{
				throw new Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.PushNotificationConfigurationException(Strings.PushNotificationEnterpriseEmptyDomain);
			}
			if (proxyPublisherSettings.ChannelSettings.ServiceUri == null)
			{
				throw new Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.PushNotificationConfigurationException(Strings.PushNotificationEnterpriseEmptyServiceUri);
			}
			tenantDomain = proxyPublisherSettings.ChannelSettings.Organization;
			targetUri = proxyPublisherSettings.ChannelSettings.ServiceUri;
		}

		protected override void ReportFailure(Exception ex)
		{
			if (ex is Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.PushNotificationConfigurationException)
			{
				base.Result.StateAttribute1 = Components.PushNotifications.ToString();
				base.Result.StateAttribute2 = ex.Message;
			}
			else if (ex is PushNotificationTransientException && ex.ContainsInnerException<SocketException>())
			{
				base.Result.StateAttribute1 = Components.Networking.ToString();
				base.Result.StateAttribute2 = Strings.PushNotificationEnterpriseNetworkingError;
			}
			else if (ex is PushNotificationPermanentException && ex.ContainsInnerException<AuthenticationException>())
			{
				base.Result.StateAttribute1 = Components.Authentication.ToString();
				base.Result.StateAttribute2 = Strings.PushNotificationEnterpriseAuthError;
			}
			else
			{
				base.Result.StateAttribute1 = Components.Unknown.ToString();
				base.Result.StateAttribute2 = Strings.PushNotificationEnterpriseUnknownError;
			}
			throw ex;
		}
	}
}
