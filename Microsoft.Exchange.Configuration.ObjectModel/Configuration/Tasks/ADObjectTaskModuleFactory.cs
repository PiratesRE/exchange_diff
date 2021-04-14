using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ADObjectTaskModuleFactory : TaskModuleFactory
	{
		public ADObjectTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.Throttling, typeof(ADResourceThrottlingModule));
		}
	}
}
