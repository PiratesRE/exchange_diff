using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	public class EnableOrganizationCustomizationTaskModuleFactory : ManageOrganizationTaskModuleFactory
	{
		public EnableOrganizationCustomizationTaskModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.Throttling, typeof(EnableOrganizationCustomizationThrottlingModule));
		}
	}
}
