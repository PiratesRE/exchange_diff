using System;

namespace Microsoft.Exchange.EDiscovery.Export
{
	public class ExportStatusEventArgs : EventArgs
	{
		public int ActualCount { get; internal set; }

		public long ActualBytes { get; internal set; }

		public int ActualMailboxesProcessed { get; internal set; }

		public int ActualMailboxesTotal { get; internal set; }

		public TimeSpan TotalDuration { get; internal set; }
	}
}
