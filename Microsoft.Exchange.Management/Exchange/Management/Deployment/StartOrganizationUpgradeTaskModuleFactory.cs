using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class StartOrganizationUpgradeTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public StartOrganizationUpgradeTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(StartOrganizationUpgradeHealthCountersModule));
		}
	}
}
