using System;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class ComponentInfoBaseThrottlingModule : ThrottlingModule<ResourceThrottlingCallback>
	{
		public ComponentInfoBaseThrottlingModule(TaskContext context) : base(context, true)
		{
		}
	}
}
