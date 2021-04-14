using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureDeviceRegistrationInfo : Notification
	{
		public AzureDeviceRegistrationInfo(string deviceId, string azureTag, string targetAppId, string serverChallenge, string hubName = null) : base(PushNotificationCannedApp.AzureDeviceRegistration.Name, deviceId, null)
		{
			this.Tag = azureTag;
			this.TargetAppId = targetAppId;
			this.HubName = hubName;
			this.ServerChallenge = serverChallenge;
		}

		[DataMember(Name = "targetAppId", EmitDefaultValue = false)]
		public string TargetAppId { get; private set; }

		[DataMember(Name = "hubName", EmitDefaultValue = false)]
		public string HubName { get; private set; }

		[DataMember(Name = "tag", EmitDefaultValue = false)]
		public string Tag { get; private set; }

		[DataMember(Name = "challenge", EmitDefaultValue = false)]
		public string ServerChallenge { get; private set; }

		public static AzureDeviceRegistrationInfo CreateMonitoringDeviceRegistrationInfo(string deviceId, string azureTag, string targetAppId, string hubName, string serverChallenge = null)
		{
			return new AzureDeviceRegistrationInfo(deviceId, azureTag, targetAppId, serverChallenge, hubName)
			{
				IsMonitoring = true
			};
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("targetAppId:").Append(this.TargetAppId.ToNullableString()).Append("; ");
			sb.Append("hubName:").Append(this.HubName.ToNullableString()).Append("; ");
			sb.Append("tag:").Append(this.Tag.ToNullableString()).Append("; ");
			sb.Append("challenge:").Append(this.ServerChallenge.ToNullableString()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.TargetAppId))
			{
				errors.Add(Strings.InvalidTargetAppId(base.GetType().Name));
			}
		}
	}
}
