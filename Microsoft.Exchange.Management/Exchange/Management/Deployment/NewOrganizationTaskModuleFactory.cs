using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class NewOrganizationTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public NewOrganizationTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(NewOrganizationHealthCountersModule));
			base.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(NewOrganizationServerSettingsModule));
		}
	}
}
