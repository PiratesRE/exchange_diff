using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public class ProgressInfo
	{
		public short Progress { get; set; }

		public TimeSpan TimeInServer { get; set; }

		public DateTime? CompletedTime { get; set; }

		public DateTime? LastExecutionTime { get; set; }

		public int CorruptionsDetected { get; set; }

		public int CorruptionsFixed { get; set; }

		public ErrorCode Error { get; set; }

		public IList<Corruption> Corruptions { get; set; }
	}
}
