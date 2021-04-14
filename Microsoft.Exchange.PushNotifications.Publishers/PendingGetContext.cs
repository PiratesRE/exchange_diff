using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PendingGetContext
	{
		internal AsyncResult AsyncResult { get; set; }

		internal IPendingGetResponse Response { get; set; }
	}
}
