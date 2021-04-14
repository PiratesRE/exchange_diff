using System;

namespace Microsoft.Exchange.Transport.RemoteDelivery
{
	internal enum ResubmitReason
	{
		Admin,
		ConfigUpdate,
		Inactivity,
		Recovery,
		UnreachableSameVersionHubs,
		Redirect,
		ShadowHeartbeatFailure,
		ShadowStateChange,
		OutboundConnectorChange
	}
}
