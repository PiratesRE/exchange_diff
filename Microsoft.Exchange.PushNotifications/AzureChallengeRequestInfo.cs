using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.PushNotifications.Wcf")]
	internal class AzureChallengeRequestInfo : Notification
	{
		public AzureChallengeRequestInfo(string targetAppId, PushNotificationPlatform platform, string deviceId, string challenge = null, string hubName = null) : base(PushNotificationCannedApp.AzureChallengeRequest.Name, deviceId, null)
		{
			this.TargetAppId = targetAppId;
			this.Platform = new PushNotificationPlatform?(platform);
			this.DeviceChallenge = challenge;
			this.HubName = hubName;
		}

		[DataMember(Name = "targetAppId", EmitDefaultValue = false)]
		public string TargetAppId { get; private set; }

		[DataMember(Name = "challenge", EmitDefaultValue = false)]
		public string DeviceChallenge { get; private set; }

		[DataMember(Name = "hubName", EmitDefaultValue = false)]
		public string HubName { get; private set; }

		[DataMember(Name = "platform", EmitDefaultValue = false)]
		public PushNotificationPlatform? Platform { get; private set; }

		public string DeviceId
		{
			get
			{
				return base.RecipientId;
			}
		}

		public static AzureChallengeRequestInfo CreateMonitoringAzureChallengeRequestInfo(string targetAppId, PushNotificationPlatform platform, string deviceId, string challenge, string hubName = null)
		{
			return new AzureChallengeRequestInfo(targetAppId, platform, deviceId, challenge, hubName)
			{
				IsMonitoring = true
			};
		}

		protected override void InternalToFullString(StringBuilder sb)
		{
			base.InternalToFullString(sb);
			sb.Append("targetAppId:").Append(this.TargetAppId.ToNullableString()).Append("; ");
			sb.Append("challenge:").Append(this.DeviceChallenge.ToNullableString()).Append("; ");
			sb.Append("hubName:").Append(this.HubName.ToNullableString()).Append("; ");
			sb.Append("platform:").Append(this.Platform.ToNullableString<PushNotificationPlatform>()).Append("; ");
		}

		protected override void InternalValidate(List<LocalizedString> errors)
		{
			base.InternalValidate(errors);
			if (string.IsNullOrWhiteSpace(this.TargetAppId))
			{
				errors.Add(Strings.InvalidTargetAppId(base.GetType().Name));
			}
			if (this.Platform != null && !this.Platform.Value.SupportsIssueRegistrationSecret())
			{
				errors.Add(Strings.InvalidPlatform);
			}
		}
	}
}
