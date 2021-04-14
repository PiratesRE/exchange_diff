using System;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class Cost : IComparable, IComparable<Cost>
	{
		internal Cost(WaitCondition condition, CostConfiguration config, Trace tracer)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.condition = condition;
			this.config = config;
			this.tracer = tracer;
		}

		internal Cost(CostConfiguration config, int threads, Trace tracer) : this(null, config, tracer)
		{
			if (threads < 0)
			{
				throw new ArgumentException("threads has to be positive", "threads");
			}
			this.usedThreads = threads;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.usedThreads == 0 && this.accessTokensCount == 0 && this.ProcessingTotal == 0L && this.startedProcessingCount == 0 && this.MemoryTotal == 0L;
			}
		}

		internal WaitCondition Condition
		{
			get
			{
				return this.condition;
			}
		}

		internal int UsedThreads
		{
			get
			{
				if (this.config.ReversedCost)
				{
					return this.config.MaxThreads - this.usedThreads - this.accessTokensCount;
				}
				return this.usedThreads + this.accessTokensCount;
			}
		}

		internal CostObjectState ObjectState
		{
			get
			{
				return this.objectState;
			}
		}

		internal long ProcessingTotal
		{
			get
			{
				if (!this.config.ProcessingHistoryEnabled || this.processingHistory == null)
				{
					if (this.config.ReversedCost)
					{
						return this.config.ProcessingCapacity;
					}
					return 0L;
				}
				else
				{
					long sum;
					lock (this.syncRoot)
					{
						sum = this.processingHistory.Sum;
					}
					if (this.config.ReversedCost)
					{
						return Math.Max(0L, this.config.ProcessingCapacity - sum);
					}
					return sum;
				}
			}
		}

		internal long MemoryTotal
		{
			get
			{
				if (!this.config.MemoryCollectionEnabled)
				{
					return 0L;
				}
				if (this.config.ReversedCost)
				{
					return Convert.ToInt64(this.config.MemoryThreshold.ToBytes());
				}
				if (this.memoryUsed == null)
				{
					return 0L;
				}
				long sum;
				lock (this.syncRoot)
				{
					sum = this.memoryUsed.Sum;
				}
				return sum;
			}
		}

		public double LastThrottleFactor
		{
			get
			{
				return this.lastThrottleFactor;
			}
		}

		public bool HasOverride
		{
			get
			{
				return this.LastThrottleFactor >= 0.0;
			}
		}

		public static bool operator <(Cost a, Cost b)
		{
			return a.CompareTo(b) == -1;
		}

		public static bool operator >(Cost a, Cost b)
		{
			return a.CompareTo(b) == 1;
		}

		public static bool operator >=(Cost a, Cost b)
		{
			return a.CompareTo(b) != -1;
		}

		public static bool operator <=(Cost a, Cost b)
		{
			return a.CompareTo(b) != 1;
		}

		public static bool operator ==(Cost a, Cost b)
		{
			return object.Equals(a, b);
		}

		public static bool operator !=(Cost a, Cost b)
		{
			return !object.Equals(a, b);
		}

		public int CompareTo(object obj)
		{
			Cost cost = (Cost)obj;
			if (cost == null)
			{
				throw new ArgumentException("obj is not of type Cost");
			}
			return this.CompareTo(cost);
		}

		public int CompareTo(Cost obj)
		{
			return this.CompareTo(this.UsedThreads, obj, obj.UsedThreads);
		}

		public int CompareWithoutAccessToken(Cost cost)
		{
			int num = this.UsedThreads;
			int otherCostUsedThreads = cost.UsedThreads;
			if (this.config.ReversedCost)
			{
				num = this.config.MaxThreads - this.usedThreads;
			}
			if (cost.config.ReversedCost)
			{
				otherCostUsedThreads = cost.config.MaxThreads - cost.usedThreads;
			}
			return this.CompareTo(num, cost, otherCostUsedThreads);
		}

		private int CompareTo(int usedThreads, Cost otherCost, int otherCostUsedThreads)
		{
			if (this.config.ReversedCost == otherCost.config.ReversedCost)
			{
				throw new InvalidOperationException("Cannot compare two reversed Cost objects or two normal Cost objects");
			}
			double num = (double)otherCostUsedThreads;
			double num2 = (double)usedThreads;
			if (otherCost.config.ReversedCost)
			{
				num = this.ApplyOverride(this, otherCostUsedThreads);
			}
			else if (this.config.ReversedCost)
			{
				num2 = this.ApplyOverride(otherCost, usedThreads);
			}
			long num3 = otherCost.config.ProcessingHistoryEnabled ? otherCost.ProcessingTotal : 0L;
			if (num2 < num && (!this.config.ProcessingHistoryEnabled || this.ProcessingTotal < num3 || (this.ProcessingTotal == 0L && this.ProcessingTotal == num3)) && (!this.config.MemoryCollectionEnabled || this.MemoryTotal < (otherCost.config.MemoryCollectionEnabled ? otherCost.MemoryTotal : 0L)))
			{
				return -1;
			}
			if (num2 == num && this.ProcessingTotal == otherCost.ProcessingTotal && this.MemoryTotal == otherCost.MemoryTotal)
			{
				return 0;
			}
			return 1;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			Cost cost = obj as Cost;
			return !(cost == null) && (this.config.ReversedCost == cost.config.ReversedCost && this.usedThreads == cost.usedThreads && this.accessTokensCount == cost.accessTokensCount && this.startedProcessingCount == cost.startedProcessingCount && ((this.processingHistory == null && cost.processingHistory == null) || (this.processingHistory != null && cost.processingHistory != null && this.processingHistory.Equals(cost.processingHistory)))) && ((this.memoryUsed == null && cost.memoryUsed == null) || (this.memoryUsed != null && cost.memoryUsed != null && this.memoryUsed.Equals(cost.memoryUsed)));
		}

		public override int GetHashCode()
		{
			return this.usedThreads.GetHashCode() ^ this.accessTokensCount.GetHashCode() ^ this.startedProcessingCount;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Used Threads: {0}, Access Tokens: {1}", this.config.ReversedCost ? this.UsedThreads : this.usedThreads, this.accessTokensCount);
			if (this.config.ProcessingHistoryEnabled)
			{
				stringBuilder.AppendFormat(", Processing Total: {0}", this.ProcessingTotal);
			}
			if (this.config.MemoryCollectionEnabled)
			{
				stringBuilder.AppendFormat(", Memory: {0}", this.MemoryTotal);
			}
			if ((this.config.OverrideEnabled || this.config.TestOverrideEnabled) && this.LastThrottleFactor != -1.0)
			{
				stringBuilder.AppendFormat(", ThrottleFactor: {0}", this.LastThrottleFactor);
			}
			return stringBuilder.ToString();
		}

		internal bool HasCapacity(bool allowAboveThreshold, int inFlightUnlocks)
		{
			if (!this.config.ReversedCost)
			{
				throw new InvalidOperationException("HasCapacity is supported on ReversedCost only");
			}
			int maxThreads = this.config.MaxThreads;
			if (allowAboveThreshold)
			{
				int maxAllowedCapacity = this.config.MaxAllowedCapacity;
				double num = (double)(this.usedThreads + this.accessTokensCount + inFlightUnlocks);
				return num / (double)maxThreads * 100.0 < (double)maxAllowedCapacity;
			}
			return this.usedThreads + this.accessTokensCount < maxThreads;
		}

		internal int GetIndexOf()
		{
			return this.usedThreads + this.accessTokensCount;
		}

		internal void RecordThreadStart()
		{
			Interlocked.Increment(ref this.usedThreads);
			this.objectState = CostObjectState.Live;
		}

		internal void RecordThreadEnd()
		{
			int num = Interlocked.Decrement(ref this.usedThreads);
			if (num < 0)
			{
				throw new InvalidOperationException("Cannot decrement used threads below zero");
			}
		}

		internal bool AddMemoryCost(ByteQuantifiedSize bytesUsed)
		{
			if (!this.config.MemoryCollectionEnabled)
			{
				return false;
			}
			if (this.config.ReversedCost)
			{
				return true;
			}
			if (bytesUsed > this.config.MinInterestingMemorySize)
			{
				lock (this.syncRoot)
				{
					if (this.memoryUsed == null)
					{
						this.memoryUsed = new SlidingTotalCounter(this.config.HistoryInterval, this.config.BucketSize, new Func<DateTime>(this.TimeProvider));
					}
					this.memoryUsed.AddValue((long)bytesUsed.ToBytes());
					this.objectState = CostObjectState.Live;
				}
			}
			return true;
		}

		internal bool MarkEmptyCostForDeletion()
		{
			if (this.objectState != CostObjectState.Init && this.IsEmpty)
			{
				this.objectState = CostObjectState.MarkedForDeletion;
				return true;
			}
			return false;
		}

		internal void MarkObjectDeleted()
		{
			this.objectState = CostObjectState.Deleted;
		}

		internal void AddToken()
		{
			Interlocked.Increment(ref this.accessTokensCount);
			this.objectState = CostObjectState.Live;
		}

		internal void ReturnToken()
		{
			Interlocked.Increment(ref this.usedThreads);
			int num = Interlocked.Decrement(ref this.accessTokensCount);
			if (num < 0)
			{
				throw new InvalidOperationException("Cannot decrement access tokens below zero");
			}
			this.objectState = CostObjectState.Live;
		}

		internal void FailToken()
		{
			int num = Interlocked.Decrement(ref this.accessTokensCount);
			if (num < 0)
			{
				throw new InvalidOperationException("Cannot decrement access tokens below zero");
			}
		}

		internal void StartProcessing()
		{
			if (!this.config.ProcessingHistoryEnabled)
			{
				return;
			}
			Interlocked.Increment(ref this.startedProcessingCount);
			this.objectState = CostObjectState.Live;
		}

		internal void CompleteProcessing(DateTime startTime)
		{
			if (!this.config.ProcessingHistoryEnabled)
			{
				return;
			}
			int num = Interlocked.Decrement(ref this.startedProcessingCount);
			if (num < 0)
			{
				throw new InvalidOperationException("Cannot complete processing something that wasn't started");
			}
			DateTime d = this.TimeProvider();
			TimeSpan t = d - startTime;
			if (t > this.config.MinInterestingProcessingInterval || this.config.ReversedCost)
			{
				lock (this.syncRoot)
				{
					if (this.processingHistory == null)
					{
						this.processingHistory = new SlidingTotalCounter(this.config.HistoryInterval, this.config.BucketSize, new Func<DateTime>(this.TimeProvider));
					}
					long value = (long)Math.Min(this.config.HistoryInterval.TotalMilliseconds, t.TotalMilliseconds);
					this.processingHistory.AddValue(value);
				}
			}
		}

		internal XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement(this.config.ReversedCost ? "FreeCost" : "Cost", new object[]
			{
				new XElement("UsedThreads", this.config.ReversedCost ? this.UsedThreads : this.usedThreads),
				new XElement("AccessTokens", this.accessTokensCount),
				new XElement("ProcessingTotal", this.ProcessingTotal),
				new XElement("MemoryTotal", this.MemoryTotal)
			});
			if (this.LastThrottleFactor != -1.0)
			{
				xelement.Add(new XElement("LastOverrideFactor", this.LastThrottleFactor));
			}
			return xelement;
		}

		private double ApplyOverride(Cost cost, int currentLimit)
		{
			if (cost.config.QuotaOverride == null || (!cost.config.OverrideEnabled && !cost.config.TestOverrideEnabled))
			{
				cost.lastThrottleFactor = -1.0;
				return (double)currentLimit;
			}
			double num = (double)currentLimit;
			ProcessingQuotaComponent.ProcessingData quotaOverride = cost.config.QuotaOverride.GetQuotaOverride(cost.condition);
			if (quotaOverride != null && quotaOverride.ThrottlingFactor < 1.0)
			{
				if (cost.config.OverrideEnabled)
				{
					num = (double)currentLimit * quotaOverride.ThrottlingFactor;
					cost.lastThrottleFactor = quotaOverride.ThrottlingFactor;
				}
				if (cost.config.TestOverrideEnabled)
				{
					this.tracer.TraceDebug<WaitCondition, double, double>((long)cost.GetHashCode(), "Quota for tenant {0} would be adjusted to {1}% and new limit is {2}", cost.condition, quotaOverride.ThrottlingFactor * 100.0, num);
				}
			}
			else
			{
				cost.lastThrottleFactor = -1.0;
			}
			return num;
		}

		private DateTime TimeProvider()
		{
			if (this.config.TimeProvider == null)
			{
				return DateTime.UtcNow;
			}
			return this.config.TimeProvider();
		}

		private readonly WaitCondition condition;

		private readonly CostConfiguration config;

		private int usedThreads;

		private int accessTokensCount;

		private int startedProcessingCount;

		private SlidingTotalCounter processingHistory;

		private SlidingTotalCounter memoryUsed;

		private double lastThrottleFactor;

		private object syncRoot = new object();

		private CostObjectState objectState;

		private Trace tracer;
	}
}
