using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Server.Services
{
	internal class DeviceBudgetWrapper : BudgetWrapper<DeviceBudget>, IDeviceBudget, IBudget, IDisposable
	{
		internal DeviceBudgetWrapper(DeviceBudget innerBudget) : base(innerBudget)
		{
		}

		public bool TryApproveSendNotification(out OverBudgetException obe)
		{
			return base.GetInnerBudget().TryApproveSendNotification(out obe);
		}

		public bool TryApproveInvalidNotification(out OverBudgetException obe)
		{
			return base.GetInnerBudget().TryApproveInvalidNotification(out obe);
		}

		protected override DeviceBudget ReacquireBudget()
		{
			return DeviceBudget.Get(base.Owner as DeviceBudgetKey);
		}
	}
}
