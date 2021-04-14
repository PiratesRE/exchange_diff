using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ResourceObserverOperation
	{
		public ResourceObserverOperation(IResourceLevelObserver resourceLevelObserver, IExecutionInfo executionInfo, int maxTransientExceptions = 5)
		{
			ArgumentValidator.ThrowIfNull("resourceLevelObserver", resourceLevelObserver);
			ArgumentValidator.ThrowIfNull("executionInfo", executionInfo);
			ArgumentValidator.ThrowIfZeroOrNegative("maxTransientExceptions", maxTransientExceptions);
			this.resourceLevelObserver = resourceLevelObserver;
			Action action = delegate()
			{
				resourceLevelObserver.HandleResourceChange(ResourceObserverOperation.allResourceUses, ResourceObserverOperation.changedResourceUses, ResourceObserverOperation.rawResourceUses);
			};
			this.operation = new Operation("HandleResourceChange method for " + resourceLevelObserver.Name, action, executionInfo, maxTransientExceptions);
		}

		public IResourceLevelObserver ResourceLevelObserver
		{
			get
			{
				return this.resourceLevelObserver;
			}
		}

		public IExecutionInfo ExecutionInfo
		{
			get
			{
				return this.operation.ExecutionInfo;
			}
		}

		public static Task InvokeResourceObserverOperations(IEnumerable<ResourceObserverOperation> resourceObserverOperations, IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses, TimeSpan timeout)
		{
			ResourceObserverOperation.allResourceUses = allResourceUses;
			ResourceObserverOperation.changedResourceUses = changedResourceUses;
			ResourceObserverOperation.rawResourceUses = rawResourceUses;
			IEnumerable<Operation> operations = from resourceObserverOperation in resourceObserverOperations
			select resourceObserverOperation.operation;
			return Operation.InvokeOperationsAsync(operations, timeout);
		}

		private const string DebugInfoPrefix = "HandleResourceChange method for ";

		private static IEnumerable<ResourceUse> allResourceUses;

		private static IEnumerable<ResourceUse> changedResourceUses;

		private static IEnumerable<ResourceUse> rawResourceUses;

		private readonly Operation operation;

		private readonly IResourceLevelObserver resourceLevelObserver;
	}
}
