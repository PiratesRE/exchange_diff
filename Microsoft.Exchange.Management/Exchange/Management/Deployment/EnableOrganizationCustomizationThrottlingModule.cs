using System;
using Microsoft.Exchange.Configuration.ObjectModel.EventLog;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class EnableOrganizationCustomizationThrottlingModule : ComponentInfoBaseThrottlingModule
	{
		public EnableOrganizationCustomizationThrottlingModule(TaskContext context) : base(context)
		{
			this.budgets.Add(new BudgetInformation
			{
				Budget = PowerShellBudget.Acquire(TenantHydrationBudgetKey.Singleton),
				ThrottledEventInfo = TaskEventLogConstants.Tuple_SlimTenantTaskThrottled
			});
		}
	}
}
