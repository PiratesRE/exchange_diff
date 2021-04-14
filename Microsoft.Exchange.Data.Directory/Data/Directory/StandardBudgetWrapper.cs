using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class StandardBudgetWrapper : StandardBudgetWrapperBase<StandardBudget>
	{
		internal StandardBudgetWrapper(StandardBudget innerBudget) : base(innerBudget)
		{
		}

		protected override StandardBudget ReacquireBudget()
		{
			return StandardBudgetCache.Singleton.Get(base.Owner);
		}
	}
}
