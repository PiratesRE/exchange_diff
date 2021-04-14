using System;
using System.IO;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal sealed class DatabaseUsedSpaceMeter : UsedDiskSpaceResourceMeter
	{
		public DatabaseUsedSpaceMeter(IMeterableJetDataSource meterableDataSource, PressureTransitions pressureTransitions) : base("DatabaseUsedSpace", DatabaseUsedSpaceMeter.GetDatabaseDir(meterableDataSource), pressureTransitions)
		{
			this.meterableDataSource = meterableDataSource;
		}

		protected override long GetCurrentPressure()
		{
			ulong num;
			ulong num2;
			ulong num3;
			if (base.GetSpace(out num, out num2, out num3))
			{
				return (long)((num2 - num - (ulong)this.meterableDataSource.GetAvailableFreeSpace()) * 100UL / num2);
			}
			return 0L;
		}

		private static string GetDatabaseDir(IMeterableJetDataSource meterableDataSource)
		{
			ArgumentValidator.ThrowIfNull("meterableDataSource", meterableDataSource);
			return Path.GetDirectoryName(meterableDataSource.DatabasePath);
		}

		private readonly IMeterableJetDataSource meterableDataSource;
	}
}
