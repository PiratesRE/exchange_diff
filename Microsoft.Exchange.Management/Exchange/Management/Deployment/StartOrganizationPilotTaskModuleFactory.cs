using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class StartOrganizationPilotTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public StartOrganizationPilotTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(StartOrganizationPilotHealthCountersModule));
		}
	}
}
