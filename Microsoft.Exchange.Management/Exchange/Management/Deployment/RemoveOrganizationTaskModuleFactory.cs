using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class RemoveOrganizationTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public RemoveOrganizationTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(RemoveOrgHealthCountersModule));
		}
	}
}
