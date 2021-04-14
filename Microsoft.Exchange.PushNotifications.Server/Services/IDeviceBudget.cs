using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal interface IDeviceBudget : IBudget, IDisposable
	{
		bool TryApproveSendNotification(out OverBudgetException obe);

		bool TryApproveInvalidNotification(out OverBudgetException obe);
	}
}
