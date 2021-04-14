using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.PushNotifications;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	internal class PushNotificationServerSubscription : PushNotificationSubscription
	{
		public PushNotificationServerSubscription()
		{
		}

		public PushNotificationServerSubscription(PushNotificationSubscription subscription, DateTime lastUpdated, string installationId)
		{
			base.AppId = subscription.AppId;
			base.DeviceNotificationId = subscription.DeviceNotificationId;
			base.DeviceNotificationType = subscription.DeviceNotificationType;
			base.InboxUnreadCount = subscription.InboxUnreadCount;
			this.LastSubscriptionUpdate = lastUpdated;
			base.SubscriptionOption = subscription.SubscriptionOption;
			this.InstallationId = installationId;
		}

		[DataMember(Name = "LastSubscriptionUpdate", EmitDefaultValue = false)]
		public DateTime LastSubscriptionUpdate { get; set; }

		[DataMember(Name = "InstallationId", EmitDefaultValue = false)]
		public string InstallationId { get; set; }

		public new static PushNotificationServerSubscription FromJson(string serializedJson)
		{
			return JsonConverter.Deserialize<PushNotificationServerSubscription>(serializedJson, null);
		}

		public override string ToJson()
		{
			return JsonConverter.Serialize<PushNotificationServerSubscription>(this, null);
		}

		public override string ToFullString()
		{
			if (this.toFullStringCache == null)
			{
				this.toFullStringCache = string.Format("{{AppId:{0}; DeviceNotificationId:{1}; DeviceNotificationType:{2}; InboxUnreadCount:{3}; LastSubscriptionUpdate:{4}; SubscriptionOption:{5}; InstallationId:{6}}}", new object[]
				{
					base.AppId ?? "null",
					base.DeviceNotificationId ?? "null",
					base.DeviceNotificationType ?? "null",
					(base.InboxUnreadCount != null) ? base.InboxUnreadCount.Value.ToString() : "null",
					this.LastSubscriptionUpdate,
					(base.SubscriptionOption != null) ? base.SubscriptionOption.Value.ToString() : "null",
					this.InstallationId ?? "null"
				});
			}
			return this.toFullStringCache;
		}

		[NonSerialized]
		private string toFullStringCache;
	}
}
