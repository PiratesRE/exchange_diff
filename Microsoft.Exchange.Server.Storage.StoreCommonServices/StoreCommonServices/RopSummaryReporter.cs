using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal sealed class RopSummaryReporter : TraceDataReporter<RopSummaryContainer>
	{
		public RopSummaryReporter(StoreDatabase database, IBinaryLogger logger, RopSummaryContainer data) : base(database, logger, data)
		{
		}

		public static TraceContextFlags GetContextFlags(StoreDatabase database)
		{
			TraceContextFlags traceContextFlags = TraceContextFlags.None;
			try
			{
				database.GetSharedLock();
				if (database.IsOnlinePassive || database.IsOnlinePassiveAttachedReadOnly || database.IsOnlinePassiveReplayingLogs)
				{
					traceContextFlags |= TraceContextFlags.Passive;
				}
			}
			finally
			{
				database.ReleaseSharedLock();
			}
			return traceContextFlags;
		}
	}
}
