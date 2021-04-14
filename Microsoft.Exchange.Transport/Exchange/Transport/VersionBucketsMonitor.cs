using System;
using System.Threading;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal sealed class VersionBucketsMonitor : ResourceMonitor
	{
		public VersionBucketsMonitor(DataSource dataSource, ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(string.Empty, configuration)
		{
			this.dataSource = dataSource;
		}

		public override void DoCleanup()
		{
			if (!this.dataSource.CleanupRequestInProgress && this.ResourceUses > ResourceUses.Normal)
			{
				this.dataSource.CleanupRequestInProgress = true;
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.dataSource.OnDataCleanup));
			}
		}

		public override string ToString(ResourceUses resourceUses, int currentPressure)
		{
			return Strings.VersionBucketUses(currentPressure, ResourceManager.MapToLocalizedString(resourceUses), base.LowPressureLimit, base.MediumPressureLimit, base.HighPressureLimit);
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			currentReading = (int)this.dataSource.GetCurrentVersionBuckets();
			return true;
		}

		private DataSource dataSource;
	}
}
