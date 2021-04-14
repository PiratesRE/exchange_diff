using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class DeviceRegistrationChallengeRequest : BaseRequest
	{
		[DataMember(Name = "AppId", IsRequired = true)]
		public string AppId { get; set; }

		[DataMember(Name = "DeviceNotificationId", IsRequired = true)]
		public string DeviceNotificationId { get; set; }

		[DataMember(Name = "ClientWatermark", IsRequired = true)]
		public string ClientWatermark { get; set; }

		[DataMember(Name = "DeviceNotificationType", IsRequired = true)]
		public string DeviceNotificationType { get; set; }

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

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RequestDeviceRegistrationChallenge(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		private PushNotificationPlatform? platform;
	}
}
