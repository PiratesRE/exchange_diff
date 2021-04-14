using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class GetTaskWithIdentityModuleFactory : GetTaskBaseModuleFactory
	{
		public GetTaskWithIdentityModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.PiiRedaction, typeof(GetWithIdentityTaskPiiRedactionModule));
		}
	}
}
