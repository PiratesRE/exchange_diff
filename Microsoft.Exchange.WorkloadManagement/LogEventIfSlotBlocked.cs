using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WorkloadManagement.EventLogs;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal class LogEventIfSlotBlocked : LogEventIf
	{
		public LogEventIfSlotBlocked(IResourceLoadMonitor monitor, ushort numberOfBuckets) : base(LogEventIfSlotBlocked.OneMinute, numberOfBuckets, numberOfBuckets / 2)
		{
			if (monitor == null)
			{
				throw new ArgumentNullException("monitor", "Monitor cannot be null.");
			}
			this.monitor = monitor;
			this.resourceKey = monitor.Key;
		}

		protected override void InternalLogEvent()
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			IResourceSettings @object = snapshot.WorkloadManagement.GetObject<IResourceSettings>(this.resourceKey.MetricType, new object[0]);
			if (@object.Enabled)
			{
				WorkloadManagerEventLogger.LogEvent(WorkloadManagementEventLogConstants.Tuple_StaleResourceMonitor, this.resourceKey.ToString(), new object[]
				{
					this.resourceKey,
					this.monitor.LastUpdateUtc,
					DateTime.UtcNow - this.monitor.LastUpdateUtc
				});
			}
		}

		private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1.0);

		private IResourceLoadMonitor monitor;

		private ResourceKey resourceKey;
	}
}
