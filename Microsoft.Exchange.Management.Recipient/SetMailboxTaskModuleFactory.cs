using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class SetMailboxTaskModuleFactory : TaskModuleFactory
	{
		public SetMailboxTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(ForestWideUserDcServerSettingsModule));
		}
	}
}
