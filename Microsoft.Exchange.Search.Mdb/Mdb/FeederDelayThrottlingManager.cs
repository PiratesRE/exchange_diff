using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class FeederDelayThrottlingManager : IFeederDelayThrottlingManager
	{
		internal FeederDelayThrottlingManager(ISearchServiceConfig config)
		{
			this.tracingContext = (long)this.GetHashCode();
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("FeederDelayThrottlingManager", ExTraceGlobals.FeederThrottlingTracer, this.tracingContext);
			this.throttlingEnabled = config.WLMThrottlingEnabled;
			if (this.throttlingEnabled)
			{
				ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.Search);
				ResourceLoadDelayInfo.Initialize();
				this.resourceHealthMonitor = ResourceHealthMonitorManager.Singleton.Get(ProcessorResourceKey.Local);
				this.mappings = default(FeederDelayThrottlingManager.ThrottlingDelayValues);
				this.mappings.Underloaded = config.RetryDelayWhenUnderloaded;
				this.mappings.Good = config.RetryDelayWhenGood;
				this.mappings.Overloaded = config.RetryDelayWhenOverloaded;
				this.mappings.Critical = config.RetryDelayWhenCritical;
				this.mappings.Unknown = TimeSpan.Zero;
			}
		}

		public TimeSpan DelayForThrottling()
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			if (this.throttlingEnabled)
			{
				ResourceLoad resourceLoad = this.resourceHealthMonitor.GetResourceLoad(WorkloadClassification.Discretionary, false, null);
				switch (resourceLoad.State)
				{
				case ResourceLoadState.Underloaded:
					timeSpan = this.mappings.Underloaded;
					goto IL_88;
				case ResourceLoadState.Full:
					timeSpan = this.mappings.Good;
					goto IL_88;
				case ResourceLoadState.Overloaded:
					timeSpan = this.mappings.Overloaded;
					goto IL_88;
				case ResourceLoadState.Critical:
					timeSpan = this.mappings.Critical;
					goto IL_88;
				}
				timeSpan = this.mappings.Unknown;
				IL_88:
				this.diagnosticsSession.Tracer.TracePerformance<ResourceLoad, TimeSpan>(this.tracingContext, "Throttling feeder with load state: {0} will be delayed for TimeSpan value: {1}", resourceLoad, timeSpan);
			}
			return timeSpan;
		}

		private const string ComponentName = "FeederDelayThrottlingManager";

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly long tracingContext;

		private readonly bool throttlingEnabled;

		private readonly IResourceLoadMonitor resourceHealthMonitor;

		private FeederDelayThrottlingManager.ThrottlingDelayValues mappings;

		private struct ThrottlingDelayValues
		{
			public TimeSpan Unknown { get; set; }

			public TimeSpan Underloaded { get; set; }

			public TimeSpan Good { get; set; }

			public TimeSpan Overloaded { get; set; }

			public TimeSpan Critical { get; set; }
		}
	}
}
