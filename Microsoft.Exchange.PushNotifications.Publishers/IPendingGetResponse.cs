using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPendingGetResponse
	{
		void Write(string payload);
	}
}
