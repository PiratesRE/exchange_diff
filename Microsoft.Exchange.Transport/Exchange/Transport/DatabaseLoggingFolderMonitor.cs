using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal sealed class DatabaseLoggingFolderMonitor : DiskSpaceMonitor
	{
		public DatabaseLoggingFolderMonitor(DataSource dataSource, ResourceManagerConfiguration.ResourceMonitorConfiguration configuration) : base(Strings.DatabaseLoggingResource(dataSource.LogFilePath), dataSource.LogFilePath, configuration, (configuration.HighThreshold == 100) ? Math.Min(Components.TransportAppConfig.JetDatabase.CheckpointDepthMax.ToBytes() * 3UL, ByteQuantifiedSize.FromGB(5UL).ToBytes()) : 0UL)
		{
		}
	}
}
