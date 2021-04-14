using System;
using System.ComponentModel;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class DiskLatencyResourceMonitor : CacheableResourceHealthMonitor, IResourceHealthPoller
	{
		public DiskLatencyResourceMonitor(DiskLatencyResourceKey key) : base(key)
		{
		}

		public TimeSpan Interval
		{
			get
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
				return snapshot.WorkloadManagement.DiskLatencySettings.ResourceHealthPollerInterval;
			}
		}

		public bool IsActive
		{
			get
			{
				VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
				return snapshot.WorkloadManagement.DiskLatency.Enabled;
			}
		}

		protected override int InternalMetricValue
		{
			get
			{
				return this.metricValue;
			}
		}

		public void Execute()
		{
			this.UpdateConfiguration();
			string databaseVolumeName = ((DiskLatencyResourceKey)base.Key).DatabaseVolumeName;
			try
			{
				if (this.lastDiskPerf == default(DiskPerformanceStructure))
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<string, string>((long)this.GetHashCode(), "[DiskLatencyResourceMonitor.Execute] MDB: {0}, Volume: {1}. First iteration, capture this.lastDiskPerf only.", base.Key.Id, databaseVolumeName);
					this.lastDiskPerf = DiskIoControl.GetDiskPerformance(databaseVolumeName);
					this.metricValue = -1;
				}
				else
				{
					int diskReadLatency = DiskIoControl.GetDiskReadLatency(databaseVolumeName, ref this.lastDiskPerf, out this.lastUpdatedTime);
					this.diskReadLatencyAverage.Add(Environment.TickCount, (uint)diskReadLatency);
					this.metricValue = (int)Math.Round((double)this.diskReadLatencyAverage.GetValue());
					if (this.lastUpdatedTime > this.LastUpdateUtc + TimeSpan.FromMilliseconds((double)(this.numberOfBuckets * this.windowBucketLength)))
					{
						this.LastUpdateUtc = TimeProvider.UtcNow;
					}
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug((long)this.GetHashCode(), "[DiskLatencyResourceMonitor.Execute] MDB: {0}, Volume: {1}. Disk read latency value is: {2}. Metric value is: {3}", new object[]
					{
						base.Key.Id,
						databaseVolumeName,
						diskReadLatency,
						this.metricValue
					});
				}
			}
			catch (Win32Exception arg)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<string, string, Win32Exception>((long)this.GetHashCode(), "[DiskLatencyResourceMonitor.Execute] MDB: {0}, Volume: {1}. Unable to read disk peformance data. Error: {2}", base.Key.Id, databaseVolumeName, arg);
				this.metricValue = -1;
			}
		}

		private static ushort ConvertIntSettingToUshort(int settingValue, ushort defaultValue)
		{
			if (0 < settingValue && settingValue < 65535)
			{
				return (ushort)settingValue;
			}
			return defaultValue;
		}

		private void UpdateConfiguration()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			IDiskLatencyMonitorSettings diskLatencySettings = snapshot.WorkloadManagement.DiskLatencySettings;
			ushort num = DiskLatencyResourceMonitor.ConvertIntSettingToUshort(diskLatencySettings.FixedTimeAverageWindowBucket.Milliseconds, 1000);
			ushort num2 = DiskLatencyResourceMonitor.ConvertIntSettingToUshort(diskLatencySettings.FixedTimeAverageNumberOfBuckets, 10);
			if (this.windowBucketLength != num || this.numberOfBuckets != num2)
			{
				this.windowBucketLength = num;
				this.numberOfBuckets = num2;
				this.diskReadLatencyAverage = new FixedTimeAverage(this.windowBucketLength, this.numberOfBuckets, Environment.TickCount);
			}
		}

		private const ushort DefaultWindowBucketLength = 1000;

		private const ushort DefaultNumberOfBuckets = 10;

		private ushort windowBucketLength;

		private ushort numberOfBuckets;

		private FixedTimeAverage diskReadLatencyAverage;

		private DateTime lastUpdatedTime;

		private DiskPerformanceStructure lastDiskPerf;

		private int metricValue = -1;
	}
}
