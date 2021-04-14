using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class PushNotificationSubscription
	{
		[DataMember(Name = "AppId", IsRequired = true)]
		public string AppId { get; set; }

		[DataMember(Name = "DeviceNotificationId", IsRequired = true)]
		public string DeviceNotificationId { get; set; }

		[DataMember(Name = "DeviceNotificationType", IsRequired = true)]
		public string DeviceNotificationType { get; set; }

		[DataMember(Name = "InboxUnreadCount", EmitDefaultValue = false)]
		public long? InboxUnreadCount { get; set; }

		[DataMember(Name = "SubscriptionOption", IsRequired = false, EmitDefaultValue = false)]
		public PushNotificationSubscriptionOption? SubscriptionOption { get; set; }

		[DataMember(Name = "RegistrationChallenge", IsRequired = false)]
		public string RegistrationChallenge { get; set; }

		public PushNotificationPlatform Platform
		{
			get
			{
				if (this.platform == null)
				{
					PushNotificationPlatform pushNotificationPlatform;
					this.platform = new PushNotificationPlatform?(Enum.TryParse<PushNotificationPlatform>(this.DeviceNotificationType, out pushNotificationPlatform) ? pushNotificationPlatform : PushNotificationPlatform.None);
				}
				return this.platform.Value;
			}
		}

		public static PushNotificationSubscription FromJson(string serializedJson)
		{
			return JsonConverter.Deserialize<PushNotificationSubscription>(serializedJson, null);
		}

		public virtual string ToJson()
		{
			return JsonConverter.Serialize<PushNotificationSubscription>(this, null);
		}

		public virtual string ToFullString()
		{
			if (this.toFullStringCache == null)
			{
				this.toFullStringCache = string.Format("{{AppId:{0}; DeviceNotificationId:{1}; DeviceNotificationType:{2}; InboxUnreadCount:{3}; SubscriptionOption:{4}; RegistrationChallenge:{5}}}", new object[]
				{
					this.AppId ?? "null",
					this.DeviceNotificationId ?? "null",
					this.DeviceNotificationType ?? "null",
					(this.InboxUnreadCount != null) ? this.InboxUnreadCount.Value.ToString() : "null",
					(this.SubscriptionOption != null) ? this.SubscriptionOption.Value.ToString() : "null",
					this.RegistrationChallenge ?? "null"
				});
			}
			return this.toFullStringCache;
		}

		public PushNotificationSubscriptionOption GetSubscriptionOption()
		{
			if (this.SubscriptionOption != null)
			{
				return this.SubscriptionOption.Value;
			}
			return PushNotificationSubscriptionOption.Email | PushNotificationSubscriptionOption.Calendar | PushNotificationSubscriptionOption.VoiceMail | PushNotificationSubscriptionOption.MissedCall | PushNotificationSubscriptionOption.BackgroundSync;
		}

		private PushNotificationPlatform? platform;

		[NonSerialized]
		private string toFullStringCache;
	}
}
