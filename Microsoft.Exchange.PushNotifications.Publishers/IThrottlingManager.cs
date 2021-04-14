using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IThrottlingManager
	{
		bool TryApproveNotification(PushNotification notification, out OverBudgetException overBudgetException);

		void ReportInvalidNotifications(PushNotification notification);
	}
}
