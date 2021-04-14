using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConnectionDroppedSubscription : BaseSubscription
	{
		public ConnectionDroppedSubscription() : base(NotificationType.ConnectionDropped)
		{
		}

		public static readonly Guid WellKnownSubscriptionId = new Guid("D83F5839-3220-4F98-BDAD-CC4AF2B6B713");
	}
}
