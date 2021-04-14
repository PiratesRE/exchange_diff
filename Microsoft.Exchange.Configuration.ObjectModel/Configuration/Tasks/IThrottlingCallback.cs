using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal interface IThrottlingCallback
	{
		void Initialize();

		void CheckResourceHealth(IThrottlingModuleInfo tmInfo);

		DelayEnforcementResults EnforceDelay(IThrottlingModuleInfo tmInfo, CostType[] costTypes, TimeSpan cmdletMaxPreferredDelay);
	}
}
