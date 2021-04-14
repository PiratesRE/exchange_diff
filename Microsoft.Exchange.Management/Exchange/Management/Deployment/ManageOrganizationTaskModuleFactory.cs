using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class ManageOrganizationTaskModuleFactory : ComponentInfoBaseTaskModuleFactory
	{
		public ManageOrganizationTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.Throttling, typeof(ADResourceThrottlingModule));
		}
	}
}
