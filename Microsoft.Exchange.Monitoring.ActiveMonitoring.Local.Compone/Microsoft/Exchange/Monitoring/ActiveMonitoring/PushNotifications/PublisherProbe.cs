using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public class PublisherProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			if (propertyBag.ContainsKey("TenantId"))
			{
				probeDefinition.Attributes["TenantId"] = propertyBag["TenantId"].ToString();
			}
			if (propertyBag.ContainsKey("IsRegistrationEnabled"))
			{
				probeDefinition.Attributes["IsRegistrationEnabled"] = propertyBag["IsRegistrationEnabled"].ToString();
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			bool flag = true;
			if (base.Definition.Attributes.ContainsKey("TenantId"))
			{
				this.tenantId = base.Definition.Attributes["TenantId"].ToString();
			}
			if (base.Definition.Attributes.ContainsKey("IsRegistrationEnabled"))
			{
				this.isRegistrationEnabledOnAzureSend = bool.Parse(base.Definition.Attributes["IsRegistrationEnabled"]);
			}
			MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory = PushNotificationsDiscovery.PublisherConfiguration.CreateMonitoringNotificationFactory(new Dictionary<PushNotificationPlatform, IMonitoringMailboxNotificationRecipientFactory>
			{
				{
					PushNotificationPlatform.APNS,
					ApnsNotificationFactory.Default
				},
				{
					PushNotificationPlatform.WNS,
					WnsNotificationFactory.Default
				},
				{
					PushNotificationPlatform.GCM,
					GcmNotificationFactory.Default
				},
				{
					PushNotificationPlatform.WebApp,
					WebAppNotificationFactory.Default
				}
			});
			try
			{
				using (PublisherServiceProxy publisherServiceProxy = new PublisherServiceProxy(null))
				{
					publisherServiceProxy.EndPublishNotifications(publisherServiceProxy.BeginPublishNotifications(monitoringMailboxNotificationFactory.CreateMonitoringNotificationBatch(), null, null));
				}
				base.Result.StateAttribute2 = "Monitoring Notification batch published for registered apps for APNS, WNS, GCM or WebApp channels";
				flag = false;
			}
			catch (InvalidOperationException ex)
			{
				if (ex.Message.Contains("The factory has no registered apps"))
				{
					base.Result.StateAttribute2 = "No registered apps found for APNS, WNS, GCM or WebApp channels; Hence no batch published.";
				}
			}
			if (this.tenantId != null)
			{
				try
				{
					MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory2 = PushNotificationsDiscovery.PublisherConfiguration.CreateMonitoringNotificationFactory(new Dictionary<PushNotificationPlatform, IMonitoringMailboxNotificationRecipientFactory>
					{
						{
							PushNotificationPlatform.Azure,
							AzureNotificationFactory.Default
						}
					});
					string deviceTokenPrefix = string.Empty;
					if (this.isRegistrationEnabledOnAzureSend)
					{
						deviceTokenPrefix = base.Result.MachineName;
					}
					using (PublisherServiceProxy publisherServiceProxy2 = new PublisherServiceProxy(null))
					{
						publisherServiceProxy2.EndPublishNotifications(publisherServiceProxy2.BeginPublishNotifications(monitoringMailboxNotificationFactory2.CreateMonitoringNotificationBatchForAzure(this.tenantId, deviceTokenPrefix), null, null));
					}
					ProbeResult result = base.Result;
					result.StateAttribute2 += " Monitoring Notification batch published for registered apps for Azure channel";
					flag = false;
				}
				catch (InvalidOperationException ex2)
				{
					if (ex2.Message.Contains("The factory has no registered apps"))
					{
						ProbeResult result2 = base.Result;
						result2.StateAttribute2 += " No registered apps found for Azure channel; Hence no batch published";
					}
				}
			}
			if (flag)
			{
				throw new InvalidOperationException("No registered apps found for any PushNotification channel. Hence no batch of monitoring notifications published");
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		public const string TenantIdProperty = "TenantId";

		public const string IsRegistrationEnabledOnAzureSendProperty = "IsRegistrationEnabled";

		protected string tenantId;

		protected bool isRegistrationEnabledOnAzureSend;
	}
}
