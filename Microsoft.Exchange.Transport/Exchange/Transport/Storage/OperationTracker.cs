using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Storage
{
	public class OperationTracker<TOperation>
	{
		public OperationTracker(Func<TimeSpan> logThreshold, Action<TOperation, TimeSpan> logAction, TimeSpan percentileValueGranularity, TimeSpan percentileValueMaximum)
		{
			this.logThreshold = logThreshold;
			this.logAction = logAction;
			this.percentileCounter = new PercentileCounter(TimeSpan.FromMinutes(15.0), TimeSpan.FromMinutes(1.0), percentileValueGranularity.Ticks, percentileValueMaximum.Ticks);
		}

		public long TotalOperationCount
		{
			get
			{
				return this.operationCount;
			}
		}

		public ICollection<OperationTracker<TOperation>.StackCounter> TracedStack
		{
			get
			{
				return this.stackCounters.Values;
			}
		}

		public void Enter(TOperation operation)
		{
			this.currentOperations.TryAdd(operation, Tuple.Create<Stopwatch, Thread>(Stopwatch.StartNew(), Thread.CurrentThread));
		}

		public void StartTracing(int operationLimit, TimeSpan traceThreshold)
		{
			this.traceLimit = 0;
			this.stackCounters.Clear();
			this.traceCount = 0;
			this.traceThreshold = traceThreshold;
			this.traceLimit = operationLimit;
		}

		public TimeSpan Exit(TOperation operation)
		{
			Tuple<Stopwatch, Thread> tuple;
			if (!this.currentOperations.TryRemove(operation, out tuple))
			{
				return TimeSpan.Zero;
			}
			Interlocked.Increment(ref this.operationCount);
			Stopwatch item = tuple.Item1;
			item.Stop();
			this.percentileCounter.AddValue(item.ElapsedTicks);
			if (this.logThreshold != null && this.logAction != null && item.Elapsed > this.logThreshold())
			{
				this.logAction(operation, item.Elapsed);
			}
			if (this.traceCount < this.traceLimit && item.Elapsed > this.traceThreshold)
			{
				Interlocked.Increment(ref this.traceCount);
				StackTrace stackTrace = new StackTrace(1, true);
				string text = stackTrace.ToString();
				int hashCode = text.GetHashCode();
				OperationTracker<TOperation>.StackCounter stackCounter;
				if (this.stackCounters.TryGetValue(hashCode, out stackCounter))
				{
					stackCounter.Increment();
				}
				else
				{
					this.stackCounters.TryAdd(hashCode, new OperationTracker<TOperation>.StackCounter(text, 1));
				}
			}
			return tuple.Item1.Elapsed;
		}

		public TimeSpan PercentileQuery(double percentage)
		{
			return TimeSpan.FromTicks(this.percentileCounter.PercentileQuery(percentage));
		}

		public IEnumerable<Tuple<TOperation, TimeSpan, StackTrace>> GetRunningOperations()
		{
			foreach (KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> operation in this.currentOperations)
			{
				KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> keyValuePair = operation;
				keyValuePair.Value.Item2.Suspend();
				Tuple<TOperation, TimeSpan, StackTrace> result;
				try
				{
					KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> keyValuePair2 = operation;
					TOperation key = keyValuePair2.Key;
					KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> keyValuePair3 = operation;
					TimeSpan elapsed = keyValuePair3.Value.Item1.Elapsed;
					KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> keyValuePair4 = operation;
					result = Tuple.Create<TOperation, TimeSpan, StackTrace>(key, elapsed, new StackTrace(keyValuePair4.Value.Item2, true));
				}
				finally
				{
					KeyValuePair<TOperation, Tuple<Stopwatch, Thread>> keyValuePair5 = operation;
					keyValuePair5.Value.Item2.Resume();
				}
				yield return result;
			}
			yield break;
		}

		private readonly ConcurrentDictionary<TOperation, Tuple<Stopwatch, Thread>> currentOperations = new ConcurrentDictionary<TOperation, Tuple<Stopwatch, Thread>>();

		private readonly ConcurrentDictionary<int, OperationTracker<TOperation>.StackCounter> stackCounters = new ConcurrentDictionary<int, OperationTracker<TOperation>.StackCounter>();

		private readonly PercentileCounter percentileCounter;

		private readonly Func<TimeSpan> logThreshold;

		private readonly Action<TOperation, TimeSpan> logAction;

		private TimeSpan traceThreshold;

		private long operationCount;

		private int traceCount;

		private volatile int traceLimit;

		public class StackCounter
		{
			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public string StackTrace { get; private set; }

			public StackCounter(string stackTrace, int initialCount)
			{
				this.StackTrace = stackTrace;
				this.count = initialCount;
			}

			public void Increment()
			{
				Interlocked.Increment(ref this.count);
			}

			private int count;
		}
	}
}
