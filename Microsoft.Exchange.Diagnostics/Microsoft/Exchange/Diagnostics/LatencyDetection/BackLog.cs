using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BackLog
	{
		internal BackLog(LatencyReportingThreshold threshold)
		{
			this.threshold = threshold;
		}

		internal int Count
		{
			get
			{
				int count;
				lock (this.lockObject)
				{
					count = this.list.Count;
				}
				return count;
			}
		}

		internal void ChangeThresholdAndClear(LatencyReportingThreshold newThreshold)
		{
			lock (this.lockObject)
			{
				this.threshold = newThreshold;
				this.Clear();
			}
		}

		internal void MoveToList(List<LatencyDetectionContext> destinationList)
		{
			lock (this.lockObject)
			{
				destinationList.AddRange(this.list);
				this.Clear();
			}
		}

		internal bool AddAndQueryThreshold(LatencyDetectionContext data)
		{
			bool flag = false;
			try
			{
				if (Monitor.TryEnter(this.lockObject))
				{
					flag = (this.countAboveThreshold >= (int)this.threshold.NumberRequired);
					if (flag)
					{
						this.Add(data);
					}
					else
					{
						this.TrimOldEntries();
						flag = this.IsLatencyAboveThreshold(data);
						if (flag)
						{
							this.SetTrigger(data);
						}
						else
						{
							this.Add(data);
						}
						this.LimitBacklogSize();
					}
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.lockObject))
				{
					Monitor.Exit(this.lockObject);
				}
			}
			return flag;
		}

		internal void Clear()
		{
			lock (this.lockObject)
			{
				this.list.Clear();
				this.countAboveThreshold = 0;
				this.trigger = null;
			}
		}

		internal bool IsBeyondThreshold(out LatencyDetectionContext triggerContext)
		{
			bool flag = false;
			triggerContext = null;
			lock (this.lockObject)
			{
				flag = (this.countAboveThreshold >= (int)this.threshold.NumberRequired);
				if (flag)
				{
					triggerContext = this.trigger;
				}
			}
			return flag;
		}

		private void SetTrigger(LatencyDetectionContext data)
		{
			this.trigger = data;
			this.IncrementCountAboveThreshold(data);
		}

		private bool IsLatencyAboveThreshold(LatencyDetectionContext data)
		{
			bool result = false;
			if ((data.TriggerOptions & TriggerOptions.DoNotTrigger) == TriggerOptions.None)
			{
				result = (data.Elapsed >= this.threshold.Threshold);
			}
			return result;
		}

		private void IncrementCountAboveThreshold(LatencyDetectionContext data)
		{
			if (this.IsLatencyAboveThreshold(data))
			{
				this.countAboveThreshold++;
			}
		}

		private void Add(LatencyDetectionContext data)
		{
			this.list.AddLast(data);
			this.IncrementCountAboveThreshold(data);
		}

		private void TrimOldEntries()
		{
			this.minToKeep = DateTime.UtcNow - BackLog.options.BacklogRetirementAge;
			while (this.list.Count > 0)
			{
				LinkedListNode<LatencyDetectionContext> first = this.list.First;
				if (!(first.Value.TimeStarted < this.minToKeep))
				{
					break;
				}
				this.RemoveNode(first);
			}
		}

		private void RemoveNode(LinkedListNode<LatencyDetectionContext> node)
		{
			LatencyDetectionContext value = node.Value;
			if (this.IsLatencyAboveThreshold(value))
			{
				this.countAboveThreshold--;
			}
			this.list.Remove(node);
		}

		private void LimitBacklogSize()
		{
			uint maximumBacklogSize = BackLog.options.MaximumBacklogSize;
			while ((long)this.list.Count > (long)((ulong)maximumBacklogSize))
			{
				this.RemoveNode(this.list.First);
			}
		}

		private static readonly PerformanceReportingOptions options = PerformanceReportingOptions.Instance;

		private readonly LinkedList<LatencyDetectionContext> list = new LinkedList<LatencyDetectionContext>();

		private readonly object lockObject = new object();

		private DateTime minToKeep = DateTime.MinValue;

		private int countAboveThreshold;

		private LatencyReportingThreshold threshold;

		private LatencyDetectionContext trigger;
	}
}
