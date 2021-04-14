using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class ProcessorResourceLoadMonitor : CacheableResourceHealthMonitor, IResourceHealthPoller
	{
		internal ProcessorResourceLoadMonitor(ProcessorResourceKey key) : base(key)
		{
			this.cpuAverage = new FixedTimeAverage(1000, (ushort)ProcessorResourceLoadMonitorConfiguration.CPUAverageTimeWindow, Environment.TickCount);
			if (CPUUsage.GetCurrentCPU(ref this.lastServerCPUUsage))
			{
				this.lastUpdatedTime = DateTime.UtcNow;
				return;
			}
			this.lastServerCPUUsage = 0L;
			this.lastUpdatedTime = DateTime.MinValue;
		}

		public bool IsActive
		{
			get
			{
				return true;
			}
		}

		public virtual TimeSpan Interval
		{
			get
			{
				return ProcessorResourceLoadMonitorConfiguration.RefreshInterval;
			}
		}

		protected override int InternalMetricValue
		{
			get
			{
				return this.metricValue;
			}
		}

		public override ResourceLoad GetResourceLoad(WorkloadType type, bool raw = false, object optionalData = null)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			IWorkloadSettings @object = snapshot.WorkloadManagement.GetObject<IWorkloadSettings>(type, new object[0]);
			if (!@object.Enabled)
			{
				return ResourceLoad.Critical;
			}
			if (!@object.EnabledDuringBlackout)
			{
				IBlackoutSettings blackout = snapshot.WorkloadManagement.Blackout;
				bool flag = blackout.StartTime != blackout.EndTime;
				if (flag)
				{
					DateTime utcNow = DateTime.UtcNow;
					DateTime t = utcNow.Date + blackout.StartTime;
					DateTime t2 = utcNow.Date + blackout.EndTime;
					if (t >= t2)
					{
						t2 = t2.AddDays(1.0);
					}
					if (t < utcNow && utcNow < t2)
					{
						return ResourceLoad.Critical;
					}
				}
			}
			return base.GetResourceLoad(type, raw, optionalData);
		}

		public virtual void Execute()
		{
			this.Update();
		}

		public override bool ShouldRemoveResourceFromCache()
		{
			return false;
		}

		private void Update()
		{
			if (!ProcessorResourceLoadMonitorConfiguration.Enabled)
			{
				this.metricValue = -1;
				return;
			}
			float num;
			if (CPUUsage.CalculateCPUUsagePercentage(ref this.lastUpdatedTime, ref this.lastServerCPUUsage, out num))
			{
				this.lastServerCPUUsagePercentage = (uint)Math.Round((double)num);
				this.cpuAverage.Add(Environment.TickCount, this.lastServerCPUUsagePercentage);
				this.metricValue = (int)Math.Round((double)this.cpuAverage.GetValue());
				if (this.lastUpdatedTime > this.LastUpdateUtc + TimeSpan.FromSeconds((double)ProcessorResourceLoadMonitorConfiguration.CPUAverageTimeWindow))
				{
					this.LastUpdateUtc = TimeProvider.UtcNow;
				}
			}
			else
			{
				this.metricValue = -1;
			}
			if (ProcessorResourceLoadMonitorConfiguration.OverrideMetricValue != null)
			{
				this.metricValue = ProcessorResourceLoadMonitorConfiguration.OverrideMetricValue.Value;
			}
		}

		private const int MillisecondsInOneSecond = 1000;

		private int metricValue = -1;

		private DateTime lastUpdatedTime;

		private long lastServerCPUUsage;

		private uint lastServerCPUUsagePercentage;

		private FixedTimeAverage cpuAverage;
	}
}
