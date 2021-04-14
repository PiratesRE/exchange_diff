using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class ForestWideUserDcServerSettingsModule : RunspaceServerSettingsInitModule
	{
		public ForestWideUserDcServerSettingsModule(TaskContext context) : base(context)
		{
		}

		protected override ADServerSettings GetCmdletADServerSettings()
		{
			bool flag = (SwitchParameter)(base.CurrentTaskContext.InvocationInfo.Fields["ForestWideDomainControllerAffinityByExecutingUser"] ?? false);
			if (flag)
			{
				return base.CreateADServerSettingsForUserWithForestWideAffnity();
			}
			return base.GetCmdletADServerSettings();
		}
	}
}
