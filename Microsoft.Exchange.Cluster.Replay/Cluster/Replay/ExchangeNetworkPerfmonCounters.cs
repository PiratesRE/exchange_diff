using System;
using Microsoft.Exchange.EseRepl;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ExchangeNetworkPerfmonCounters
	{
		internal ExchangeNetworkPerfmonCounters(NetworkManagerPerfmonInstance instance)
		{
			this.m_instance = instance;
		}

		public void RecordLogCopyThruputReceived(long bytesCopied)
		{
			this.m_instance.LogCopyThruputReceived.IncrementBy(bytesCopied / 1024L);
		}

		public void RecordSeederThruputReceived(long bytesCopied)
		{
			this.m_instance.SeederThruputReceived.IncrementBy(bytesCopied / 1024L);
		}

		public void RecordCompressedDataReceived(int compressedSize, int decompressedSize, NetworkPath.ConnectionPurpose connectionPurpose)
		{
			switch (connectionPurpose)
			{
			case NetworkPath.ConnectionPurpose.Seeding:
				this.m_instance.TotalCompressedSeedingBytesReceived.IncrementBy((long)compressedSize);
				this.m_instance.TotalSeedingBytesDecompressed.IncrementBy((long)decompressedSize);
				return;
			case NetworkPath.ConnectionPurpose.LogCopy:
				this.m_instance.TotalCompressedLogBytesReceived.IncrementBy((long)compressedSize);
				this.m_instance.TotalLogBytesDecompressed.IncrementBy((long)decompressedSize);
				return;
			default:
				return;
			}
		}

		private NetworkManagerPerfmonInstance m_instance;
	}
}
