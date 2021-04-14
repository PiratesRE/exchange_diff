using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncHealthLogger : ISyncHealthLog
	{
		private SyncHealthLogger()
		{
		}

		public static SyncHealthLogger Instance
		{
			get
			{
				return SyncHealthLogger.instance;
			}
		}

		public void LogWorkTypeBudgets(KeyValuePair<string, object>[] eventData)
		{
			SyncHealthLogManager.LogWorkTypeBudgets(eventData);
		}

		private static SyncHealthLogger instance = new SyncHealthLogger();
	}
}
