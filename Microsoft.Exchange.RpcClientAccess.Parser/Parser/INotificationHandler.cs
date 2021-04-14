using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface INotificationHandler
	{
		bool HasPendingNotifications();

		void CollectNotifications(NotificationCollector collector);

		void RegisterCallback(Action callback);

		void CancelCallback();
	}
}
