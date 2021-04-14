using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public class NewMailboxTaskModuleFactory : TaskModuleFactory
	{
		public NewMailboxTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(NewMailboxHealthCountersModule));
			base.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(ForestWideUserDcServerSettingsModule));
		}
	}
}
