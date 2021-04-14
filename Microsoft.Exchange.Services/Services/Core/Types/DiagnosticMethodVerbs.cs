using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class DiagnosticMethodVerbs
	{
		public const string GetActiveSubscriptionIds = "GetActiveSubscriptionIds";

		public const string ClearSubscriptions = "ClearSubscriptions";

		public const string GetHangingSubscriptionConnections = "GetHangingSubscriptionConnections";

		public const string SetStreamingSubscriptionTimeToLiveDefault = "SetStreamingSubscriptionTimeToLiveDefault";

		public const string SetStreamingSubscriptionNewEventQueueSize = "SetStreamingSubscriptionNewEventQueueSize";

		public const string SetStreamingConnectionHeartbeatDefault = "SetStreamingConnectionHeartbeatDefault";

		public const string GetStreamingSubscriptionExpirationTime = "GetStreamingSubscriptionExpirationTime";

		public const string ClearExchangeRunspaceConfigurationCache = "ClearExchangeRunspaceConfigurationCache";
	}
}
