using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class CompleteOrganizationUpgradeTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public CompleteOrganizationUpgradeTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(CompleteOrganizationUpgradeHealthCountersModule));
		}
	}
}
