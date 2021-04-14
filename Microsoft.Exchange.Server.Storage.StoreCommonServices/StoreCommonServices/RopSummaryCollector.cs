using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class RopSummaryCollector : TraceCollector<RopSummaryAggregator, RopSummaryContainer, RopTraceKey, RopSummaryParameters>, IRopSummaryCollector, ITraceCollector<RopTraceKey, RopSummaryParameters>
	{
		public RopSummaryCollector(StoreDatabase database) : base(database, LoggerType.RopSummary)
		{
		}

		internal static void Initialize()
		{
			if (RopSummaryCollector.ropSummaryCollectorDatabaseSlot == -1)
			{
				RopSummaryCollector.ropSummaryCollectorDatabaseSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void MountHandler(Context context)
		{
			context.Database.ComponentData[RopSummaryCollector.ropSummaryCollectorDatabaseSlot] = new RopSummaryCollector(context.Database);
		}

		internal static IRopSummaryCollector GetRopSummaryCollector(Context context)
		{
			return RopSummaryCollector.GetRopSummaryCollector(context.Database);
		}

		internal static IRopSummaryCollector GetRopSummaryCollector(StoreDatabase database)
		{
			if (database != null)
			{
				return (RopSummaryCollector)database.ComponentData[RopSummaryCollector.ropSummaryCollectorDatabaseSlot];
			}
			return RopSummaryCollector.Null;
		}

		public static IRopSummaryCollector Null = new RopSummaryCollector.NullSummaryCollector();

		private static int ropSummaryCollectorDatabaseSlot = -1;

		internal class NullSummaryCollector : IRopSummaryCollector, ITraceCollector<RopTraceKey, RopSummaryParameters>
		{
			public void Add(RopTraceKey key, RopSummaryParameters parameters)
			{
			}
		}
	}
}
