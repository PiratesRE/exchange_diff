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
	public class AzureChallengeRequestProbe : ProbeWorkItem
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
			if (!propertyBag.ContainsKey("TenantId"))
			{
				throw new ArgumentException("Please specify value for TenantId");
			}
			probeDefinition.Attributes["TenantId"] = propertyBag["TenantId"].ToString();
			if (propertyBag.ContainsKey("AppPlatform"))
			{
				probeDefinition.Attributes["AppPlatform"] = propertyBag["AppPlatform"].ToString();
				return;
			}
			throw new ArgumentException("Please specify value for AppPlatform");
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
			if (base.Definition.Attributes.ContainsKey("AppPlatform"))
			{
				this.appPlatform = base.Definition.Attributes["AppPlatform"].ToString();
			}
			MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory = PushNotificationsDiscovery.PublisherConfiguration.CreateMonitoringNotificationFactory(new Dictionary<PushNotificationPlatform, IMonitoringMailboxNotificationRecipientFactory>
			{
				{
					PushNotificationPlatform.Azure,
					AzureNotificationFactory.Default
				}
			});
			string challenge = Guid.NewGuid().ToString();
			string monitoringDeviceToken = monitoringMailboxNotificationFactory.GetMonitoringDeviceToken(base.Result.MachineName, this.targetAppId);
			PushNotificationPlatform platform = (PushNotificationPlatform)Enum.Parse(typeof(PushNotificationPlatform), this.appPlatform, true);
			using (AzureChallengeRequestServiceProxy azureChallengeRequestServiceProxy = new AzureChallengeRequestServiceProxy(null))
			{
				azureChallengeRequestServiceProxy.EndChallengeRequest(azureChallengeRequestServiceProxy.BeginChallengeRequest(AzureChallengeRequestInfo.CreateMonitoringAzureChallengeRequestInfo(this.targetAppId, platform, monitoringDeviceToken, challenge, this.tenantId), null, null));
			}
		}

		internal override IEnumerable<PropertyInformation> GetSubstitutePropertyInformation()
		{
			return new List<PropertyInformation>();
		}

		public const string TargetAppIdProperty = "TargetAppId";

		public const string TenantIdProperty = "TenantId";

		public const string AppPlatformProperty = "AppPlatform";

		protected string targetAppId;

		protected string tenantId;

		protected string appPlatform;
	}
}
