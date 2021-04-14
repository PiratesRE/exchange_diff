using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class SetADPermissionTaskModuleFactory : TaskModuleFactory
	{
		public SetADPermissionTaskModuleFactory()
		{
			base.UnregisterModule(TaskModuleKey.AutoReportProgress);
		}
	}
}
