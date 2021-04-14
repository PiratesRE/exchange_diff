using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.ManagementEndpoint
{
	public class GetManagementEndpointTaskModuleFactory : TaskModuleFactory
	{
		public GetManagementEndpointTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(GetManagementEndpointHealthCountersModule));
		}
	}
}
