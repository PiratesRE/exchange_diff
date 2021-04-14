using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal sealed class ResourceTrackingOperation : Operation
	{
		public ResourceTrackingOperation(IResourceMeter resourceMeter, IExecutionInfo executionInfo, int maxTransientExceptions = 5) : base(ResourceTrackingOperation.GetDebugInfo(resourceMeter), ResourceTrackingOperation.GetResourceTrackingAction(resourceMeter), executionInfo, maxTransientExceptions)
		{
			this.resourceMeter = resourceMeter;
		}

		public IResourceMeter ResourceMeter
		{
			get
			{
				return this.resourceMeter;
			}
		}

		private static Action GetResourceTrackingAction(IResourceMeter resourceMeter)
		{
			ArgumentValidator.ThrowIfNull("resourceMeter", resourceMeter);
			return new Action(resourceMeter.Refresh);
		}

		private static string GetDebugInfo(IResourceMeter resourceMeter)
		{
			ArgumentValidator.ThrowIfNull("resourceMeter", resourceMeter);
			return "Refresh method for " + resourceMeter.Resource;
		}

		private const string DebugInfoPrefix = "Refresh method for ";

		private readonly IResourceMeter resourceMeter;
	}
}
