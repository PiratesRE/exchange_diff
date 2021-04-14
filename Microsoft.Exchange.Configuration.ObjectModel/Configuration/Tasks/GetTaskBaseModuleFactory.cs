using System;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class GetTaskBaseModuleFactory : TaskModuleFactory
	{
		public GetTaskBaseModuleFactory()
		{
			base.RegisterModule(TaskModuleKey.PiiRedaction, typeof(GetTaskPiiRedactionModule));
		}
	}
}
