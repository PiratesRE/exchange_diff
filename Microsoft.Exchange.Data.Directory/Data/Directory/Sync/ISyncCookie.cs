using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface ISyncCookie
	{
		Dictionary<string, int> ErrorObjectsAndFailureCounts { get; }

		DateTime SequenceStartTimestamp { get; }

		Guid SequenceId { get; }
	}
}
