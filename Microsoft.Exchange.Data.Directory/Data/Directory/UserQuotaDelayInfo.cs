using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory
{
	internal class UserQuotaDelayInfo : DelayInfo
	{
		public UserQuotaDelayInfo(TimeSpan delay, OverBudgetException exception, bool required) : base(delay, required)
		{
			this.OverBudgetException = exception;
		}

		public static UserQuotaDelayInfo CreateInfinite(OverBudgetException exception)
		{
			return new UserQuotaDelayInfo(Budget.IndefiniteDelay, exception, true);
		}

		public OverBudgetException OverBudgetException { get; private set; }
	}
}
