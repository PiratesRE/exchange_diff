using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class MissingHubEventArgs : EventArgs
	{
		public MissingHubEventArgs(string targetAppId, string hubName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("targetAppId", targetAppId);
			ArgumentValidator.ThrowIfNullOrWhiteSpace("hubName", hubName);
			this.TargetAppId = targetAppId;
			this.HubName = hubName;
		}

		public string HubName { get; private set; }

		public string TargetAppId { get; private set; }
	}
}
