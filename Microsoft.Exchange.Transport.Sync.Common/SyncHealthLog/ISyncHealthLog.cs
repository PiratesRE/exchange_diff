using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface ISyncHealthLog
	{
		void LogWorkTypeBudgets(KeyValuePair<string, object>[] eventData);
	}
}
