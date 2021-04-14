using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPendingGetConnectionCache
	{
		bool TryGetConnection(string connectionId, out IPendingGetConnection connection);

		IPendingGetConnection AddOrGetConnection(string connectionId);
	}
}
