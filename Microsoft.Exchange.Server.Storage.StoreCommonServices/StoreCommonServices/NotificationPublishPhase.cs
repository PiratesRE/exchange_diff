using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public enum NotificationPublishPhase : uint
	{
		PreCommit = 2U,
		PostCommit = 4U,
		Pumping = 8U
	}
}
