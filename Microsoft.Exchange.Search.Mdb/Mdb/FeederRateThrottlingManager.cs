using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class FeederRateThrottlingManager : IFeederRateThrottlingManager
	{
		internal FeederRateThrottlingManager(ISearchServiceConfig config, MdbInfo mdbInfo, FeederRateThrottlingManager.ThrottlingRateExecutionType execType) : this(config, mdbInfo, execType, null)
		{
			IResourceLoadMonitor resourceLoadMonitor = null;
			if (this.throttlingEnabled)
			{
				ResourceHealthMonitorManager.Initialize(ResourceHealthComponent.Search);
				ResourceLoadDelayInfo.Initialize();
				resourceLoadMonitor = ResourceHealthMonitorManager.Singleton.Get(ProcessorResourceKey.Local);
			}
			this.resourceHealthMonitor = resourceLoadMonitor;
		}

		internal FeederRateThrottlingManager(ISearchServiceConfig config, MdbInfo mdbInfo, FeederRateThrottlingManager.ThrottlingRateExecutionType execType, IResourceLoadMonitor resourceHealthMonitor)
		{
			this.tracingContext = (long)this.GetHashCode();
			this.diagnosticsSession = DiagnosticsSession.CreateComponentDiagnosticsSession("FeederRateThrottlingManager", ExTraceGlobals.FeederThrottlingTracer, this.tracingContext);
			this.execType = execType;
			this.mdbInfo = mdbInfo;
			this.mappings = default(FeederRateThrottlingManager.ThrottlingRateValues);
			switch (this.execType)
			{
			case FeederRateThrottlingManager.ThrottlingRateExecutionType.Fast:
				this.mappings.StartRate = config.CrawlerRateFast;
				this.mappings.Max = config.CrawlerRateMaxFast;
				this.mappings.Min = config.CrawlerRateMinFast;
				this.mappings.Cpu = 100.0;
				break;
			case FeederRateThrottlingManager.ThrottlingRateExecutionType.LowResource:
				this.mappings.StartRate = config.CrawlerRateLowResource;
				this.mappings.Max = config.CrawlerRateMaxLowResource;
				this.mappings.Min = config.CrawlerRateMinLowResource;
				this.mappings.Cpu = (double)config.CrawlerRateCpuLowResource;
				break;
			}
			this.throttlingEnabled = config.WLMThrottlingEnabled;
			this.resourceHealthMonitor = resourceHealthMonitor;
		}

		public virtual double ThrottlingRateContinue(double currentRate)
		{
			double num = this.mappings.StartRate;
			if (this.throttlingEnabled)
			{
				ResourceLoad load = this.resourceHealthMonitor.GetResourceLoad((this.execType == FeederRateThrottlingManager.ThrottlingRateExecutionType.Fast) ? WorkloadClassification.CustomerExpectation : WorkloadClassification.Discretionary, false, null);
				this.diagnosticsSession.TraceDebug<MdbInfo, double>("(MDB {0}): Inital LoadRatio is {1}", this.mdbInfo, load.LoadRatio);
				load = this.CalibrateResourceLoad(load);
				this.diagnosticsSession.TraceDebug<MdbInfo, double, ResourceLoadState>("(MDB {0}): Adjusted LoadRatio is {1} with state {2}", this.mdbInfo, load.LoadRatio, load.State);
				this.lastUpdateTime = ((this.lastUpdateTime == this.lastChangeTime) ? this.resourceHealthMonitor.LastUpdateUtc : this.lastUpdateTime);
				switch (load.State)
				{
				case ResourceLoadState.Underloaded:
					if (this.lastUpdateTime != this.lastChangeTime && this.lastUpdateTime != this.resourceHealthMonitor.LastUpdateUtc)
					{
						if (currentRate != 0.0)
						{
							double num2 = Math.Max(1.0 + (1.0 - load.LoadRatio) / (2.0 * load.LoadRatio), 1.01);
							num = currentRate * num2;
						}
						else
						{
							num = ((this.mappings.Min > 0.0) ? this.mappings.Min : 0.2);
						}
						this.lastChangeTime = this.resourceHealthMonitor.LastUpdateUtc;
						this.lastUpdateTime = this.lastChangeTime;
						goto IL_22D;
					}
					num = currentRate;
					goto IL_22D;
				case ResourceLoadState.Overloaded:
					if (this.lastUpdateTime != this.lastChangeTime && this.lastUpdateTime != this.resourceHealthMonitor.LastUpdateUtc)
					{
						double num3 = Math.Max(0.9 / load.LoadRatio, 0.1);
						num = currentRate * num3;
						this.lastChangeTime = this.resourceHealthMonitor.LastUpdateUtc;
						this.lastUpdateTime = this.lastChangeTime;
						goto IL_22D;
					}
					num = currentRate;
					goto IL_22D;
				case ResourceLoadState.Critical:
					num = 0.0;
					goto IL_22D;
				}
				num = currentRate;
				IL_22D:
				if (ResourceLoadState.Critical != load.State)
				{
					num = Math.Min(num, this.mappings.Max);
					num = Math.Max(num, this.mappings.Min);
				}
				this.diagnosticsSession.TraceDebug("(MDB {0}): Throttling feeder rate. Load state is {1} and rate will be {2} doc/sec from previous value {3} doc/sec", new object[]
				{
					this.mdbInfo,
					load.State,
					num,
					currentRate
				});
				this.diagnosticsSession.Tracer.TracePerformance(this.tracingContext, "(MDB {0}): Throttling feeder rate. Load state is {1} and rate will be {2} doc/sec from previous value {3} doc/sec", new object[]
				{
					this.mdbInfo,
					load.State,
					num,
					currentRate
				});
			}
			return num;
		}

		public virtual double ThrottlingRateStart()
		{
			double currentRate = this.mappings.StartRate;
			if (this.throttlingEnabled)
			{
				this.lastUpdateTime = this.resourceHealthMonitor.LastUpdateUtc;
				this.lastChangeTime = this.resourceHealthMonitor.LastUpdateUtc;
				ResourceLoad resourceLoad = this.resourceHealthMonitor.GetResourceLoad((this.execType == FeederRateThrottlingManager.ThrottlingRateExecutionType.Fast) ? WorkloadClassification.CustomerExpectation : WorkloadClassification.Discretionary, false, null);
				switch (this.CalibrateResourceLoad(resourceLoad).State)
				{
				case ResourceLoadState.Underloaded:
					currentRate = this.mappings.StartRate;
					goto IL_B0;
				case ResourceLoadState.Critical:
					currentRate = 0.0;
					goto IL_B0;
				}
				currentRate = this.mappings.Min;
			}
			IL_B0:
			return this.ThrottlingRateContinue(currentRate);
		}

		private ResourceLoad CalibrateResourceLoad(ResourceLoad load)
		{
			ResourceLoad full;
			if (load.LoadRatio / (this.mappings.Cpu / 100.0) >= 0.95 && load.LoadRatio / (this.mappings.Cpu / 100.0) <= 1.05)
			{
				full = ResourceLoad.Full;
			}
			else if (load.LoadRatio < ResourceLoad.Critical.LoadRatio - (100.0 - this.mappings.Cpu) / 100.0)
			{
				full = new ResourceLoad(load.LoadRatio + (100.0 - this.mappings.Cpu) / 100.0, null, null);
			}
			else
			{
				full = new ResourceLoad(load.LoadRatio, null, null);
			}
			return full;
		}

		private const string ComponentName = "FeederRateThrottlingManager";

		private readonly IDiagnosticsSession diagnosticsSession;

		private readonly long tracingContext;

		private readonly bool throttlingEnabled;

		private readonly IResourceLoadMonitor resourceHealthMonitor;

		private readonly FeederRateThrottlingManager.ThrottlingRateExecutionType execType;

		private readonly MdbInfo mdbInfo;

		private readonly FeederRateThrottlingManager.ThrottlingRateValues mappings;

		private DateTime lastUpdateTime;

		private DateTime lastChangeTime;

		public enum ThrottlingRateExecutionType
		{
			Fast,
			LowResource
		}

		private struct ThrottlingRateValues
		{
			public double StartRate { get; set; }

			public double Max { get; set; }

			public double Min { get; set; }

			public double Cpu { get; set; }
		}
	}
}
