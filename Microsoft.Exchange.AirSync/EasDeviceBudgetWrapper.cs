using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.AirSync
{
	internal class EasDeviceBudgetWrapper : StandardBudgetWrapperBase<EasDeviceBudget>, IEasDeviceBudget, IStandardBudget, IBudget, IDisposable
	{
		internal EasDeviceBudgetWrapper(EasDeviceBudget innerBudget) : base(innerBudget)
		{
		}

		protected override EasDeviceBudget ReacquireBudget()
		{
			return EasDeviceBudgetCache.Singleton.Get(base.Owner);
		}

		public void AddInteractiveCall()
		{
			base.GetInnerBudget().AddInteractiveCall();
		}

		public void AddCall()
		{
			base.GetInnerBudget().AddCall();
		}

		public float Percentage
		{
			get
			{
				return base.GetInnerBudget().Percentage;
			}
		}
	}
}
