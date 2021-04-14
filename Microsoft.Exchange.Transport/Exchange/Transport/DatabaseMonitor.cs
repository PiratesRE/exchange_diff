using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal class DatabaseMonitor : DiskSpaceMonitor
	{
		public DatabaseMonitor(DataSource dataSource, ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(Strings.DatabaseResource(dataSource.DatabasePath), dataSource.DatabasePath, configuration, (configuration.HighThreshold == 100) ? ByteQuantifiedSize.FromMB(500UL).ToBytes() : 0UL)
		{
			this.dataSource = dataSource;
		}

		protected override ulong GetFreeBytesAvailableOffset()
		{
			return this.dataSource.GetAvailableFreeSpace();
		}

		private DataSource dataSource;
	}
}
