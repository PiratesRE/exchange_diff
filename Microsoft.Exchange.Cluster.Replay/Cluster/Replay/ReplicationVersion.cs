using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	[Serializable]
	internal class ReplicationVersion
	{
		public static readonly ReplicationVersion CurrentVersion = new ReplicationVersion
		{
			Major = 2,
			Minor = 0
		};

		public int Major;

		public int Minor;
	}
}
