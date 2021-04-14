using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestPayloadWriter
	{
		public void AddPlatform(PushNotificationPlatform platform)
		{
			ArgumentValidator.ThrowIfInvalidValue<PushNotificationPlatform>("platform", platform, (PushNotificationPlatform x) => x.SupportsIssueRegistrationSecret());
			this.targetPlatform = AzureChallengeRequestPayloadWriter.PlatformMapping[platform];
		}

		public void AddDeviceId(string deviceId)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("deviceId", deviceId);
			this.deviceId = deviceId;
		}

		public void AddChallenge(string challenge)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("challenge", challenge);
			this.deviceChallenge = challenge;
		}

		public override string ToString()
		{
			return string.Format("{{\"Channel\":\"{0}\",\"ApplicationPlatform\":\"{1}\",\"DeviceChallenge\":\"{2}\"}}", this.deviceId, this.targetPlatform.ToString(), this.deviceChallenge);
		}

		private const string RequestBodyTemplate = "{{\"Channel\":\"{0}\",\"ApplicationPlatform\":\"{1}\",\"DeviceChallenge\":\"{2}\"}}";

		private static readonly Dictionary<PushNotificationPlatform, AzureChallengeRequestPayloadWriter.IssueSecretPlatform> PlatformMapping = new Dictionary<PushNotificationPlatform, AzureChallengeRequestPayloadWriter.IssueSecretPlatform>
		{
			{
				PushNotificationPlatform.APNS,
				AzureChallengeRequestPayloadWriter.IssueSecretPlatform.apple
			},
			{
				PushNotificationPlatform.WNS,
				AzureChallengeRequestPayloadWriter.IssueSecretPlatform.windows
			},
			{
				PushNotificationPlatform.GCM,
				AzureChallengeRequestPayloadWriter.IssueSecretPlatform.gcm
			}
		};

		private string deviceChallenge;

		private AzureChallengeRequestPayloadWriter.IssueSecretPlatform targetPlatform;

		private string deviceId;

		private enum IssueSecretPlatform
		{
			windows,
			apple,
			gcm,
			windowsphone,
			adm
		}
	}
}
