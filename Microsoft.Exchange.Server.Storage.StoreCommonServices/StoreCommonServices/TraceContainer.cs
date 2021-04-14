using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class TraceContainer<TAggregator, TKey, TParameters> where TAggregator : TraceDataAggregator<TParameters> where TParameters : ITraceParameters
	{
		public TraceContainer()
		{
			this.data = new ConcurrentDictionary<TKey, TAggregator>(20, 10000);
			this.startTimeStamp = new Stopwatch();
			this.startTimeStamp.Start();
		}

		internal bool HasDataToLog
		{
			get
			{
				return this.hasData && this.startTimeStamp.ToTimeSpan() > this.TraceInterval;
			}
		}

		internal virtual TimeSpan TraceInterval
		{
			get
			{
				return TimeSpan.FromMinutes(5.0);
			}
		}

		internal IEnumerable<KeyValuePair<TKey, TAggregator>> Data
		{
			get
			{
				return this.data;
			}
		}

		internal void Set(TKey key, TParameters parameters)
		{
			if (parameters.HasDataToLog)
			{
				TAggregator orAdd;
				if (!this.data.TryGetValue(key, out orAdd))
				{
					orAdd = this.data.GetOrAdd(key, this.CreateEmptyAggregator());
				}
				this.UpdateAggregator(orAdd, parameters);
				this.hasData = true;
			}
		}

		internal void Commit(StoreDatabase database, IBinaryLogger logger)
		{
			if (this.hasData && Interlocked.Exchange(ref this.governor, 1) == 0)
			{
				this.WriteTrace(database, logger);
			}
		}

		internal abstract TAggregator CreateEmptyAggregator();

		internal abstract TAggregator UpdateAggregator(TAggregator aggregator, TParameters parameters);

		internal abstract void WriteTrace(StoreDatabase database, IBinaryLogger logger);

		private readonly Stopwatch startTimeStamp;

		private readonly ConcurrentDictionary<TKey, TAggregator> data;

		private bool hasData;

		private int governor;
	}
}
