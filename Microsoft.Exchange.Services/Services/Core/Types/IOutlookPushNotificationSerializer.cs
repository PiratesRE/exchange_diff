using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal abstract class IOutlookPushNotificationSerializer
	{
		internal abstract byte[] ConvertToPayloadForHxCalendar(PendingOutlookPushNotification notification);

		internal abstract byte[] ConvertToPayloadForHxMail(PendingOutlookPushNotification notification);
	}
}
