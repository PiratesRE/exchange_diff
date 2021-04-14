using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class AddSecondaryDomainTaskModuleFactory : ComponentInfoBaseTaskModuleFactory
	{
		public AddSecondaryDomainTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.CmdletHealthCounters, typeof(AddSecondaryDomainHealthCountersModule));
			base.RegisterModule(TaskModuleKey.RunspaceServerSettingsInit, typeof(AddSecondaryDomainServerSettingsModule));
		}
	}
}
