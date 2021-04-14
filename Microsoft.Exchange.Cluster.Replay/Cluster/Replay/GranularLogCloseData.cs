using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class GranularLogCloseData
	{
		public GranularLogCloseData.ChecksumAlgorithm ChecksumUsed;

		public long Generation;

		public DateTime LastWriteUtc;

		public byte[] ChecksumBytes;

		public enum ChecksumAlgorithm : uint
		{
			None,
			MD5,
			Sha1
		}
	}
}
