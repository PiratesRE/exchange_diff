using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NotificationBrokerClientLogger : ExtensibleLogger
	{
		public NotificationBrokerClientLogger() : base(new NotificationBrokerClientLogConfiguration())
		{
		}
	}
}
