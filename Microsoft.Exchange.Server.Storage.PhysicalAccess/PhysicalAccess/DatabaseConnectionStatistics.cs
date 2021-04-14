using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class DatabaseConnectionStatistics : IExecutionTrackingData<DatabaseConnectionStatistics>
	{
		public DatabaseConnectionStatistics()
		{
			this.dumpedRowStats.Initialize();
			this.RowStats.Initialize();
		}

		public int Count { get; set; }

		public TimeSpan TotalTime
		{
			get
			{
				return this.TimeInDatabase;
			}
		}

		public void Reset()
		{
			this.TimeInDatabase = TimeSpan.Zero;
			this.Count = 0;
			this.ThreadStats = default(JET_THREADSTATS);
			this.RowStats.Reset();
			this.OffPageBlobHits = 0;
			this.dumpedRowStats.Reset();
			this.dumpedOffPageBlobHits = 0;
		}

		public void Aggregate(DatabaseConnectionStatistics dataToAggregate)
		{
			this.TimeInDatabase += dataToAggregate.TimeInDatabase;
			this.Count += dataToAggregate.Count;
			this.ThreadStats += dataToAggregate.ThreadStats;
			this.RowStats.Aggregate(dataToAggregate.RowStats);
			this.OffPageBlobHits += dataToAggregate.OffPageBlobHits;
		}

		public void AppendToTraceContentBuilder(TraceContentBuilder cb)
		{
			cb.Append(((long)this.TimeInDatabase.TotalMicroseconds()).ToString("N0", CultureInfo.InvariantCulture));
			cb.Append(" us");
			DatabaseOperationStatistics.AppendThreadStatsToTraceContentBuilder(ref this.ThreadStats, cb);
			if (!this.RowStats.IsEmpty)
			{
				cb.Append(", STOR:[");
				this.RowStats.AppendToString(cb);
				cb.Append("]");
			}
			if (this.OffPageBlobHits != 0)
			{
				cb.Append(", opg:[");
				cb.Append(this.OffPageBlobHits);
				cb.Append("]");
			}
		}

		public void AppendDetailsToTraceContentBuilder(TraceContentBuilder cb, int indentLevel)
		{
		}

		internal void DumpStatistics(Database database)
		{
			if (database != null && database.PerfInstance != null)
			{
				if (this.RowStats.DumpStats(database.PerfInstance, this.dumpedRowStats))
				{
					this.dumpedRowStats.CopyFrom(this.RowStats);
				}
				if (this.dumpedOffPageBlobHits != this.OffPageBlobHits)
				{
					database.PerfInstance.OffPageBlobHitsPerSec.IncrementBy((long)((ulong)(this.OffPageBlobHits - this.dumpedOffPageBlobHits)));
					this.dumpedOffPageBlobHits = this.OffPageBlobHits;
				}
			}
		}

		private RowStats dumpedRowStats;

		private int dumpedOffPageBlobHits;

		public JET_THREADSTATS ThreadStats;

		public RowStats RowStats;

		public int OffPageBlobHits;

		public TimeSpan TimeInDatabase;
	}
}
