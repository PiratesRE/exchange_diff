using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Configuration.Tasks
{
	internal class ADResourceThrottlingModule : ThrottlingModule<ResourceThrottlingCallback>
	{
		public ADResourceThrottlingModule(TaskContext context) : base(context)
		{
			Array.Resize<ResourceKey>(ref this.resourceKeys, this.resourceKeys.Length + 1);
			this.resourceKeys[this.resourceKeys.Length - 1] = ADResourceKey.Key;
		}
	}
}
