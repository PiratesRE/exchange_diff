using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class MeterableJetDataSourceFactory
	{
		internal static Func<DataSource, IMeterableJetDataSource> CreateMeterableJetDataSourceFunc
		{
			get
			{
				return MeterableJetDataSourceFactory.createMeterableDataSourceFunc;
			}
			set
			{
				MeterableJetDataSourceFactory.createMeterableDataSourceFunc = value;
			}
		}

		internal static IMeterableJetDataSource CreateMeterableDataSource(DataSource dataSource)
		{
			return MeterableJetDataSourceFactory.CreateMeterableJetDataSourceFunc(dataSource);
		}

		private static IMeterableJetDataSource CreateRealMeterableJetDataSource(DataSource dataSource)
		{
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			return new MeterableJetDataSource(dataSource);
		}

		private static Func<DataSource, IMeterableJetDataSource> createMeterableDataSourceFunc = new Func<DataSource, IMeterableJetDataSource>(MeterableJetDataSourceFactory.CreateRealMeterableJetDataSource);
	}
}
