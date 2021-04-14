using System;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal enum DelayedAckStatus
	{
		None,
		Stamped,
		ShadowRedundancyManagerNotified,
		WaitingForShadowRedundancyManager
	}
}
