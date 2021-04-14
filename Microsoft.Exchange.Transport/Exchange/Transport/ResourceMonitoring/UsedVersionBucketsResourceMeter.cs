using System;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal sealed class UsedVersionBucketsResourceMeter : ResourceMeter
	{
		public UsedVersionBucketsResourceMeter(IMeterableJetDataSource meterableDataSourcee, PressureTransitions pressureTransitions) : base("UsedVersionBuckets", UsedVersionBucketsResourceMeter.GetDatabasePath(meterableDataSourcee), pressureTransitions)
		{
			this.meterableDataSource = meterableDataSourcee;
		}

		protected override long GetCurrentPressure()
		{
			return this.meterableDataSource.GetCurrentVersionBuckets();
		}

		private static string GetDatabasePath(IMeterableJetDataSource meterableDataSource)
		{
			ArgumentValidator.ThrowIfNull("meterableDataSource", meterableDataSource);
			return meterableDataSource.DatabasePath;
		}

		private readonly IMeterableJetDataSource meterableDataSource;
	}
}
