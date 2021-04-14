using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal interface IThrottlingModuleInfo
	{
		IPowerShellBudget PSBudget { get; }

		WorkloadSettings GetWorkloadSettings();

		ResourceKey[] ResourceKeys { get; }

		bool OnBeforeDelay(DelayInfo delayInfo);
	}
}
