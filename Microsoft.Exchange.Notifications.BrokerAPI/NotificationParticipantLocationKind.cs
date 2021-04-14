using System;

namespace Microsoft.Exchange.Notifications.Broker
{
	[Serializable]
	public enum NotificationParticipantLocationKind
	{
		Unknown,
		LocalResourceForest,
		RemoteResourceForest,
		CrossPremise
	}
}
