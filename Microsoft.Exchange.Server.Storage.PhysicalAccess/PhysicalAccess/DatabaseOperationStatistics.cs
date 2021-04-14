using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class DatabaseOperationStatistics : IExecutionTrackingData<DatabaseOperationStatistics>
	{
		public DatabaseOperationStatistics()
		{
			this.SmallRowStats.Initialize();
		}

		public int Count { get; set; }

		public TimeSpan TotalTime
		{
			get
			{
				return this.TimeInDatabase;
			}
		}

		public void Aggregate(DatabaseOperationStatistics dataToAggregate)
		{
			this.TimeInDatabase += dataToAggregate.TimeInDatabase;
			this.Count += dataToAggregate.Count;
			this.ThreadStats += dataToAggregate.ThreadStats;
			this.SmallRowStats.Aggregate(dataToAggregate.SmallRowStats);
			this.OffPageBlobHits += dataToAggregate.OffPageBlobHits;
			this.Planner = ((this.Planner != null) ? this.Planner : dataToAggregate.Planner);
		}

		public void AppendToTraceContentBuilder(TraceContentBuilder cb)
		{
			cb.Append(((long)this.TimeInDatabase.TotalMicroseconds()).ToString("N0", CultureInfo.InvariantCulture));
			cb.Append(" us");
			DatabaseOperationStatistics.AppendThreadStatsToTraceContentBuilder(ref this.ThreadStats, cb);
			if (!this.SmallRowStats.IsEmpty)
			{
				cb.Append(", STOR:[");
				this.SmallRowStats.AppendToString(cb);
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
			if (this.Planner != null)
			{
				this.Planner.AppendToTraceContentBuilder(cb, indentLevel, "DB plan steps:");
			}
		}

		internal static void AppendThreadStatsToTraceContentBuilder(ref JET_THREADSTATS threadStats, TraceContentBuilder cb)
		{
			if (threadStats.cPageReferenced != 0 || threadStats.cPageRead != 0 || threadStats.cPagePreread != 0 || threadStats.cPageDirtied != 0 || threadStats.cPageRedirtied != 0 || threadStats.cLogRecord != 0 || threadStats.cbLogRecord != 0)
			{
				cb.Append(", JET:[");
				bool flag = false;
				if (threadStats.cPageReferenced != 0)
				{
					cb.Append("ref:");
					cb.Append(threadStats.cPageReferenced);
					flag = true;
				}
				if (threadStats.cPageRead != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("rd:");
					cb.Append(threadStats.cPageRead);
					flag = true;
				}
				if (threadStats.cPagePreread != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("prd:");
					cb.Append(threadStats.cPagePreread);
					flag = true;
				}
				if (threadStats.cPageDirtied != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("dt:");
					cb.Append(threadStats.cPageDirtied);
					flag = true;
				}
				if (threadStats.cPageRedirtied != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("rdt:");
					cb.Append(threadStats.cPageRedirtied);
					flag = true;
				}
				if (threadStats.cLogRecord != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("clg:");
					cb.Append(threadStats.cLogRecord);
					flag = true;
				}
				if (threadStats.cbLogRecord != 0)
				{
					if (flag)
					{
						cb.Append(", ");
					}
					cb.Append("blg:");
					cb.Append((uint)threadStats.cbLogRecord);
				}
				cb.Append("]");
			}
		}

		public JET_THREADSTATS ThreadStats;

		public SmallRowStats SmallRowStats;

		public int OffPageBlobHits;

		public TimeSpan TimeInDatabase;

		public IExecutionPlanner Planner;
	}
}
