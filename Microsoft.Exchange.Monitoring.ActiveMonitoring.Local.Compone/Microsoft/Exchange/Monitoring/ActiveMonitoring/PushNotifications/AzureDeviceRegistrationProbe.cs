using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.PushNotifications;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.Publishers;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PushNotifications
{
	public class AzureDeviceRegistrationProbe : ProbeWorkItem
	{
		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probeDefinition = definition as ProbeDefinition;
			if (probeDefinition == null)
			{
				throw new ArgumentException("definition must be a ProbeDefinition");
			}
			if (!propertyBag.ContainsKey("TargetAppId"))
			{
				throw new ArgumentException("Please specify value for TargetAppIdMask");
			}
			probeDefinition.Attributes["TargetAppId"] = propertyBag["TargetAppId"].ToString();
			if (propertyBag.ContainsKey("TenantId"))
			{
				probeDefinition.Attributes["TenantId"] = propertyBag["TenantId"].ToString();
				return;
			}
			throw new ArgumentException("Please specify value for TenantId");
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (base.Definition.Attributes.ContainsKey("TargetAppId"))
			{
				this.targetAppId = base.Definition.Attributes["TargetAppId"].ToString();
			}
			if (base.Definition.Attributes.ContainsKey("TenantId"))
			{
				this.tenantId = base.Definition.Attributes["TenantId"].ToString();
			}
			MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory = PushNotificationsDiscovery.PublisherConfiguration.CreateMonitoringNotificationFactory(new Dictionary<PushNotificationPlatform, IMonitoringMailboxNotificationRecipientFactory>
			{
				{
					PushNotificationPlatform.Azure,
					AzureNotificationFactory.Default
				}
			});
			string azureTag = Guid.NewGuid().ToString();
			string monitoringDeviceToken = monitoringMailboxNotificationFactory.GetMonitoringDeviceToken(base.Result.MachineName, this.targetAppId);
			using (AzureDeviceRegistrationServiceProxy azureDeviceRegistrationServiceProxy = new AzureDeviceRegistrationServiceProxy(null))
			{
				azureDeviceRegistrationServiceProxy.EndDeviceRegistration(azureDeviceRegistrationServiceProxy.BeginDeviceRegistration(AzureDeviceRegistrationInfo.CreateMonitoringDeviceRegistrationInfo(monitoringDeviceToken, azureTag, this.targetAppId, this.tenantId, null), null, null));
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		public const string TargetAppIdProperty = "TargetAppId";

		public const string TenantIdProperty = "TenantId";

		protected string targetAppId;

		protected string tenantId;
	}
}
