using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal sealed class SetGroupMailboxTaskModuleFactory : ADObjectTaskModuleFactory
	{
		public SetGroupMailboxTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(ForestWideTenantDcServerSettingsModule));
		}
	}
}
