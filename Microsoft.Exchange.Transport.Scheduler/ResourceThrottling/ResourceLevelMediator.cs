using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ResourceLevelMediator
	{
		public ResourceLevelMediator(IResourceTracker resourceTracker, IEnumerable<IResourceLevelObserver> resourceLevelObservers, TimeSpan operationTimeout, int maxTransientExceptions = 5)
		{
			ArgumentValidator.ThrowIfNull("resourceTracker", resourceTracker);
			ArgumentValidator.ThrowIfNull("resourceLevelObservers", resourceLevelObservers);
			ArgumentValidator.ThrowIfInvalidValue<IEnumerable<IResourceLevelObserver>>("resourceLevelObservers", resourceLevelObservers, (IEnumerable<IResourceLevelObserver> observers) => observers.Any<IResourceLevelObserver>());
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("operationTimeout", operationTimeout, (TimeSpan timeout) => timeout > TimeSpan.Zero);
			ArgumentValidator.ThrowIfZeroOrNegative("maxTransientExceptions", maxTransientExceptions);
			this.resourceLevelObservers = resourceLevelObservers;
			this.operationTimeout = operationTimeout;
			foreach (IResourceLevelObserver resourceLevelObserver in resourceLevelObservers)
			{
				if (resourceLevelObserver != null)
				{
					if (this.resourceObserverOperations.ContainsKey(resourceLevelObserver.Name))
					{
						throw new ArgumentException(string.Format("Duplicate Resource Level Observer : {0}", resourceLevelObserver.Name), "resourceLevelObservers");
					}
					DelegatingInfoCollector executionInfo = new DelegatingInfoCollector(new List<IExecutionInfo>
					{
						new ExecutionTimeInfo()
					});
					this.resourceObserverOperations.Add(resourceLevelObserver.Name, new ResourceObserverOperation(resourceLevelObserver, executionInfo, maxTransientExceptions));
				}
			}
			resourceTracker.ResourceUseChanged += this.OnResourceUseChanged;
		}

		public IEnumerable<IResourceLevelObserver> ResourceLevelObservers
		{
			get
			{
				return this.resourceLevelObservers;
			}
		}

		public Task OnResourceUseChanged(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			return ResourceObserverOperation.InvokeResourceObserverOperations(this.resourceObserverOperations.Values, allResourceUses, changedResourceUses, rawResourceUses, this.operationTimeout);
		}

		public XElement GetDiagnosticInfo(bool verbose)
		{
			if (verbose)
			{
				this.UpdateDiagnostics();
			}
			XElement xelement = new XElement("ResourceLevelMediator");
			foreach (IResourceLevelObserver resourceLevelObserver in this.resourceLevelObservers)
			{
				XElement xelement2 = new XElement("ResourceLevelObserver", new XAttribute("Name", resourceLevelObserver.Name));
				xelement2.Add(new XElement("Paused", resourceLevelObserver.Paused));
				xelement2.Add(new XElement("SubStatus", resourceLevelObserver.SubStatus));
				if (verbose)
				{
					xelement2.Add(new XElement("CallDuration", this.diagnosticsData.GetResourceObserverCallDuration(resourceLevelObserver.Name).TotalMilliseconds));
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		public ResourceLevelMediatorDiagnosticsData GetDiagnosticsData()
		{
			this.UpdateDiagnostics();
			return this.diagnosticsData;
		}

		private void UpdateDiagnostics()
		{
			foreach (string text in this.resourceObserverOperations.Keys)
			{
				DelegatingInfoCollector delegatingInfoCollector = this.resourceObserverOperations[text].ExecutionInfo as DelegatingInfoCollector;
				foreach (IExecutionInfo executionInfo in delegatingInfoCollector.ExecutionInfos)
				{
					ExecutionTimeInfo executionTimeInfo = executionInfo as ExecutionTimeInfo;
					if (executionTimeInfo != null)
					{
						this.diagnosticsData.SetResourceObserverCallDuration(text, executionTimeInfo.CallDuration);
					}
				}
			}
		}

		private readonly IEnumerable<IResourceLevelObserver> resourceLevelObservers;

		private readonly TimeSpan operationTimeout;

		private Dictionary<string, ResourceObserverOperation> resourceObserverOperations = new Dictionary<string, ResourceObserverOperation>();

		private ResourceLevelMediatorDiagnosticsData diagnosticsData = new ResourceLevelMediatorDiagnosticsData();
	}
}
