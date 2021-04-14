using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class ForestWideTenantDcServerSettingsModule : RunspaceServerSettingsInitModule
	{
		public ForestWideTenantDcServerSettingsModule(TaskContext context) : base(context)
		{
		}

		protected override ADServerSettings GetCmdletADServerSettings()
		{
			return base.CreateADServerSettingsForOrganization(true);
		}
	}
}
