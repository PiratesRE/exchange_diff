using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class ComponentInfoBaseTaskModuleFactory : TaskModuleFactory
	{
		public ComponentInfoBaseTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CollectCmdletLogEntries, typeof(CollectCmdletLogEntriesModule));
			base.RegisterModule(TaskModuleKey.Throttling, typeof(ComponentInfoBaseThrottlingModule));
		}
	}
}
