using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LastLogInfo
	{
		public long ClusterLastLogGen { get; set; }

		public DateTime ClusterLastLogTime { get; set; }

		public Exception ClusterLastLogException { get; set; }

		public bool ClusterTimeIsMissing { get; set; }

		public long ReplLastLogGen { get; set; }

		public DateTime ReplLastLogTime { get; set; }

		public bool IsStale { get; set; }

		public DateTime StaleCheckTime { get; set; }

		public long LastLogGenToReport { get; set; }

		public DateTime CollectionTime { get; set; }
	}
}
