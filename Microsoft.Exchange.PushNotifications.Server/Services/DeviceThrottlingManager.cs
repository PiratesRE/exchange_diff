using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Publishers;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class DeviceThrottlingManager : IThrottlingManager
	{
		public bool TryApproveNotification(PushNotification notification, out OverBudgetException overBudgetException)
		{
			return this.TryApproveNotification(notification, DeviceThrottlingManager.ThrottlingType.Send, out overBudgetException);
		}

		public void ReportInvalidNotifications(PushNotification notification)
		{
			OverBudgetException ex;
			if (!this.TryApproveNotification(notification, DeviceThrottlingManager.ThrottlingType.Invalid, out ex))
			{
				string text = (ex != null) ? ex.ToTraceString() : string.Empty;
				PushNotificationTracker.ReportDropped(notification, text);
				PushNotificationsCrimsonEvents.DeviceOverBudget.LogPeriodic<string, string, string>(notification.RecipientId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.ToFullString(), text);
			}
		}

		private bool TryApproveNotification(PushNotification notification, DeviceThrottlingManager.ThrottlingType throttlingType, out OverBudgetException overBudgetException)
		{
			DeviceBudgetKey budgetKey = new DeviceBudgetKey(notification.RecipientId, notification.TenantId);
			bool result;
			using (IDeviceBudget deviceBudget = DeviceBudget.Acquire(budgetKey))
			{
				switch (throttlingType)
				{
				case DeviceThrottlingManager.ThrottlingType.Send:
					result = deviceBudget.TryApproveSendNotification(out overBudgetException);
					break;
				case DeviceThrottlingManager.ThrottlingType.Invalid:
					result = deviceBudget.TryApproveInvalidNotification(out overBudgetException);
					break;
				default:
					throw new NotSupportedException(throttlingType.ToString());
				}
			}
			return result;
		}

		public static readonly DeviceThrottlingManager Default = new DeviceThrottlingManager();

		private enum ThrottlingType
		{
			Send,
			Invalid
		}
	}
}
