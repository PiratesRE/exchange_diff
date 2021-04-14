using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class RemoveSecondaryDomainTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public RemoveSecondaryDomainTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(RemoveSecondaryDomainHealthCountersModule));
		}
	}
}
