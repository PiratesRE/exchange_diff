using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ResourceThrottlingCallback : IThrottlingCallback
	{
		public void Initialize()
		{
			ResourceLoadDelayInfo.Initialize();
		}

		public void CheckResourceHealth(IThrottlingModuleInfo tmInfo)
		{
			ResourceLoadDelayInfo.CheckResourceHealth(tmInfo.PSBudget, tmInfo.GetWorkloadSettings(), tmInfo.ResourceKeys);
		}

		public DelayEnforcementResults EnforceDelay(IThrottlingModuleInfo tmInfo, CostType[] costTypes, TimeSpan cmdletMaxPreferredDelay)
		{
			return ResourceLoadDelayInfo.EnforceDelay(tmInfo.PSBudget, tmInfo.GetWorkloadSettings(), costTypes, tmInfo.ResourceKeys, cmdletMaxPreferredDelay, new Func<DelayInfo, bool>(tmInfo.OnBeforeDelay));
		}
	}
}
