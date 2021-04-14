using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestPayload
	{
		public AzureChallengeRequestPayload(PushNotificationPlatform platform, string deviceChallenge)
		{
			this.DeviceChallenge = deviceChallenge;
			this.TargetPlatform = platform;
		}

		public string DeviceChallenge { get; private set; }

		public PushNotificationPlatform TargetPlatform { get; private set; }

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Format("{{challenge:{0}; platform:{1}}}", this.DeviceChallenge, this.TargetPlatform);
			}
			return this.toString;
		}

		internal void WriteAzureSecretRequestPayload(AzureChallengeRequestPayloadWriter apw)
		{
			ArgumentValidator.ThrowIfNull("apw", apw);
			if (!string.IsNullOrWhiteSpace(this.DeviceChallenge))
			{
				apw.AddChallenge(this.DeviceChallenge);
			}
			apw.AddPlatform(this.TargetPlatform);
		}

		private string toString;
	}
}
