using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class ResourceTracker : IResourceTracker
	{
		public ResourceTracker(IEnumerable<IResourceMeter> resourceMeters, TimeSpan trackingInterval, TimeSpan operationTimeout, ResourceLog resourceLog, TimeSpan logInterval, Func<IEnumerable<ResourceUse>, UseLevel> aggregationFunc, int maxTransientExceptions = 5)
		{
			ArgumentValidator.ThrowIfNull("resourceMeters", resourceMeters);
			ArgumentValidator.ThrowIfInvalidValue<IEnumerable<IResourceMeter>>("resourceMeters", resourceMeters, (IEnumerable<IResourceMeter> meters) => meters.Any<IResourceMeter>());
			ArgumentValidator.ThrowIfNull("resourceLog", resourceLog);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("logInterval", logInterval, (TimeSpan timeout) => timeout > TimeSpan.Zero);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("operationTimeout", operationTimeout, (TimeSpan timeout) => timeout > TimeSpan.Zero);
			ArgumentValidator.ThrowIfInvalidValue<TimeSpan>("trackingInterval", trackingInterval, (TimeSpan interval) => interval > TimeSpan.Zero);
			ArgumentValidator.ThrowIfZeroOrNegative("maxTransientExceptions", maxTransientExceptions);
			if (aggregationFunc != null)
			{
				this.aggregationFunc = aggregationFunc;
			}
			this.operationTimeout = operationTimeout;
			this.trackingInterval = trackingInterval;
			this.resourceLog = resourceLog;
			this.logInterval = logInterval;
			this.isTracking = false;
			this.resourceMeters = resourceMeters;
			this.currentResourseUses = new List<ResourceUse>
			{
				new ResourceUse(this.AggregateResourceUse.Resource, UseLevel.Low, UseLevel.Low)
			};
			this.lastUpdateTime = DateTime.MinValue;
			foreach (IResourceMeter resourceMeter in resourceMeters)
			{
				if (resourceMeter != null)
				{
					if (this.resourceTrackingOperations.ContainsKey(resourceMeter.Resource))
					{
						throw new ArgumentException(string.Format("Duplicate Resource Meter for {0} : {1}", resourceMeter.Resource.Name, resourceMeter.Resource.InstanceName), "resourceMeters");
					}
					DelegatingInfoCollector executionInfo = new DelegatingInfoCollector(new List<IExecutionInfo>
					{
						new ExecutionTimeInfo()
					});
					ResourceTrackingOperation value = new ResourceTrackingOperation(resourceMeter, executionInfo, maxTransientExceptions);
					this.resourceTrackingOperations.Add(resourceMeter.Resource, value);
				}
			}
		}

		public event ResourceUseChangedHandler ResourceUseChanged;

		public ResourceUse AggregateResourceUse
		{
			get
			{
				return this.aggregateResourceUse;
			}
		}

		public bool IsTracking
		{
			get
			{
				return this.isTracking;
			}
		}

		public DateTime LastUpdateTime
		{
			get
			{
				return this.lastUpdateTime;
			}
		}

		public IEnumerable<IResourceMeter> ResourceMeters
		{
			get
			{
				return this.resourceMeters;
			}
		}

		public IEnumerable<ResourceUse> ResourceUses
		{
			get
			{
				return this.currentResourseUses;
			}
		}

		public TimeSpan TrackingInterval
		{
			get
			{
				return this.trackingInterval;
			}
		}

		[DebuggerStepThrough]
		public Task StartResourceTrackingAsync(CancellationToken cancellationToken)
		{
			ResourceTracker.<StartResourceTrackingAsync>d__d <StartResourceTrackingAsync>d__d;
			<StartResourceTrackingAsync>d__d.<>4__this = this;
			<StartResourceTrackingAsync>d__d.cancellationToken = cancellationToken;
			<StartResourceTrackingAsync>d__d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartResourceTrackingAsync>d__d.<>1__state = -1;
			AsyncTaskMethodBuilder <>t__builder = <StartResourceTrackingAsync>d__d.<>t__builder;
			<>t__builder.Start<ResourceTracker.<StartResourceTrackingAsync>d__d>(ref <StartResourceTrackingAsync>d__d);
			return <StartResourceTrackingAsync>d__d.<>t__builder.Task;
		}

		public async Task TrackResourceUseAsync()
		{
			if (this.resourceTrackingOperations.Any<KeyValuePair<ResourceIdentifier, ResourceTrackingOperation>>())
			{
				IList<ResourceUse> updatedResourceUses = await this.GetUpdatedResourceUsesAsync();
				UseLevel tempAggregateUseLevel = this.aggregationFunc(updatedResourceUses);
				this.aggregateResourceUse = new ResourceUse(this.aggregateResourceUse.Resource, tempAggregateUseLevel, this.aggregateResourceUse.CurrentUseLevel);
				updatedResourceUses.Add(this.aggregateResourceUse);
				this.currentResourseUses = updatedResourceUses;
				this.lastUpdateTime = DateTime.UtcNow;
				IEnumerable<ResourceUse> changedResourceUses = from resourceUse in this.currentResourseUses
				where resourceUse.CurrentUseLevel != resourceUse.PreviousUseLevel
				select resourceUse;
				IEnumerable<ResourceUse> changedRawResourceUses = from resourceUse in this.currentRawResourseUses
				where resourceUse.CurrentUseLevel != resourceUse.PreviousUseLevel
				select resourceUse;
				if (changedResourceUses.Any<ResourceUse>() || changedRawResourceUses.Any<ResourceUse>())
				{
					this.LogResourceChange(changedResourceUses);
					await this.RaiseResourceUseChangedEventAsync(changedResourceUses, this.currentRawResourseUses);
				}
				else
				{
					this.LogPeriodicResourceUse();
				}
			}
		}

		public XElement GetDiagnosticInfo(bool verbose)
		{
			XElement xelement = new XElement("ResourceTracker");
			if (verbose)
			{
				this.UpdateDiagnostics();
			}
			if (this.aggregateResourceUse != null)
			{
				xelement.Add(new XElement("AggregateUse", this.aggregateResourceUse.CurrentUseLevel));
			}
			foreach (IResourceMeter resourceMeter in this.resourceMeters)
			{
				XElement xelement2 = new XElement("ResourceMeter", new XAttribute("Resource", resourceMeter.Resource.ToString()));
				xelement2.Add(new XElement("CurrentResourceUse", resourceMeter.ResourceUse.CurrentUseLevel));
				xelement2.Add(new XElement("PreviousResourceUse", resourceMeter.ResourceUse.PreviousUseLevel));
				xelement2.Add(new XElement("PressureTransitions", resourceMeter.PressureTransitions));
				xelement2.Add(new XElement("Pressure", resourceMeter.Pressure));
				if (verbose)
				{
					xelement2.Add(new XElement("CallDuration", this.diagnosticsData.GetResourceMeterCallDuration(resourceMeter.Resource).TotalMilliseconds));
				}
				xelement.Add(xelement2);
			}
			return xelement;
		}

		public ResourceTrackerDiagnosticsData GetDiagnosticsData()
		{
			return this.diagnosticsData;
		}

		private void LogPeriodicResourceUse()
		{
			if (DateTime.UtcNow - this.lastLogTime >= this.logInterval)
			{
				foreach (ResourceUse resourceUse in this.currentResourseUses)
				{
					this.resourceLog.LogResourceUsePeriodic(resourceUse, null);
				}
				this.lastLogTime = DateTime.UtcNow;
			}
		}

		private void LogResourceChange(IEnumerable<ResourceUse> changedResourceUses)
		{
			using (IEnumerator<ResourceUse> enumerator = changedResourceUses.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ResourceUse changedResourceUse = enumerator.Current;
					Dictionary<string, object> dictionary = null;
					if (changedResourceUse.Resource.Name != "Aggregate")
					{
						PressureTransitions pressureTransitions = this.resourceMeters.Single((IResourceMeter resourceMeter) => resourceMeter.Resource.Equals(changedResourceUse.Resource)).PressureTransitions;
						dictionary = new Dictionary<string, object>();
						dictionary.Add("PressureTransition", pressureTransitions.ToString());
					}
					this.resourceLog.LogResourceUseChange(changedResourceUse, dictionary);
					this.lastLogTime = DateTime.UtcNow;
				}
			}
		}

		private async Task<IList<ResourceUse>> GetUpdatedResourceUsesAsync()
		{
			await Operation.InvokeOperationsAsync(this.resourceTrackingOperations.Values, this.operationTimeout);
			this.currentRawResourseUses = from operation in this.resourceTrackingOperations.Values
			select operation.ResourceMeter.RawResourceUse;
			this.UpdateDiagnostics();
			return (from operation in this.resourceTrackingOperations.Values
			select operation.ResourceMeter.ResourceUse).ToList<ResourceUse>();
		}

		private void UpdateDiagnostics()
		{
			foreach (ResourceIdentifier resourceIdentifier in this.resourceTrackingOperations.Keys)
			{
				DelegatingInfoCollector delegatingInfoCollector = this.resourceTrackingOperations[resourceIdentifier].ExecutionInfo as DelegatingInfoCollector;
				foreach (IExecutionInfo executionInfo in delegatingInfoCollector.ExecutionInfos)
				{
					ExecutionTimeInfo executionTimeInfo = executionInfo as ExecutionTimeInfo;
					if (executionTimeInfo != null)
					{
						this.diagnosticsData.SetResourceMeterCallDuration(resourceIdentifier, executionTimeInfo.CallDuration);
					}
				}
			}
		}

		private async Task RaiseResourceUseChangedEventAsync(IEnumerable<ResourceUse> changedResourceUses, IEnumerable<ResourceUse> rawResourceUses)
		{
			ResourceUseChangedHandler resourceUseChanged = this.ResourceUseChanged;
			if (resourceUseChanged != null)
			{
				await Task.WhenAll(from ResourceUseChangedHandler handler in resourceUseChanged.GetInvocationList()
				select handler(this.currentResourseUses, changedResourceUses, rawResourceUses));
			}
		}

		private readonly TimeSpan operationTimeout;

		private readonly Dictionary<ResourceIdentifier, ResourceTrackingOperation> resourceTrackingOperations = new Dictionary<ResourceIdentifier, ResourceTrackingOperation>();

		private readonly TimeSpan trackingInterval;

		private readonly ResourceLog resourceLog;

		private readonly TimeSpan logInterval;

		private DateTime lastLogTime = DateTime.MinValue;

		private ResourceUse aggregateResourceUse = new ResourceUse(new ResourceIdentifier("Aggregate", ""), UseLevel.Low, UseLevel.Low);

		private IEnumerable<ResourceUse> currentResourseUses;

		private IEnumerable<ResourceUse> currentRawResourseUses;

		private bool isTracking;

		private DateTime lastUpdateTime;

		private ResourceTrackerDiagnosticsData diagnosticsData = new ResourceTrackerDiagnosticsData();

		private IEnumerable<IResourceMeter> resourceMeters;

		private Func<IEnumerable<ResourceUse>, UseLevel> aggregationFunc = delegate(IEnumerable<ResourceUse> resourceUses)
		{
			if (resourceUses.Any<ResourceUse>())
			{
				return resourceUses.Max((ResourceUse use) => use.CurrentUseLevel);
			}
			return UseLevel.Low;
		};
	}
}
