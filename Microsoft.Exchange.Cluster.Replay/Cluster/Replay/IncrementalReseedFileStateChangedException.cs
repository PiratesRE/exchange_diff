using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class IncrementalReseedFileStateChangedException : IncrementalReseedRetryableException
	{
		public IncrementalReseedFileStateChangedException() : base(string.Empty)
		{
		}
	}
}
