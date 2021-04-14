using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.SyncHealthLog;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class WorkTypeBudgetManager : DisposeTrackableBase
	{
		internal WorkTypeBudgetManager(SyncLogSession syncLogSession, ISyncHealthLog syncHealthLog, ISyncManagerConfiguration configuration)
		{
			this.slidingWindowLength = configuration.WorkTypeBudgetManagerSlidingWindowLength;
			this.slidingBucketLength = configuration.WorkTypeBudgetManagerSlidingBucketLength;
			this.sampleDispatchedWorkFrequency = configuration.WorkTypeBudgetManagerSampleDispatchedWorkFrequency;
			this.syncLogSession = syncLogSession;
			this.dispatchedEntriesPerWorkType = new Dictionary<WorkType, int>();
			this.rollingAverageDispatchedEntries = new Dictionary<WorkType, SlidingAverageCounter>();
			this.cachedTotalSyncTime = 0L;
			this.syncHealthLogger = syncHealthLog;
			this.sampleTimer = new GuardedTimer(new TimerCallback(this.SampleDispatchedWork), null, (int)this.sampleDispatchedWorkFrequency.TotalMilliseconds, -1);
		}

		internal void Increment(WorkType workType)
		{
			this.collectionLock.EnterWriteLock();
			try
			{
				if (!this.dispatchedEntriesPerWorkType.ContainsKey(workType))
				{
					this.dispatchedEntriesPerWorkType.Add(workType, 1);
				}
				else
				{
					Dictionary<WorkType, int> dictionary;
					(dictionary = this.dispatchedEntriesPerWorkType)[workType] = dictionary[workType] + 1;
				}
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
		}

		internal void Decrement(WorkType workType)
		{
			this.collectionLock.EnterWriteLock();
			try
			{
				if (this.dispatchedEntriesPerWorkType.ContainsKey(workType))
				{
					Dictionary<WorkType, int> dictionary;
					(dictionary = this.dispatchedEntriesPerWorkType)[workType] = dictionary[workType] - 1;
				}
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
			}
		}

		internal virtual bool HasBudget(WorkType workType)
		{
			this.collectionLock.EnterReadLock();
			bool result;
			try
			{
				result = (this.GetPercentOfBudgetUsed(workType) < 100.0);
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return result;
		}

		internal XElement GetDiagnosticInfo()
		{
			XElement xelement = new XElement("WorkTypeBudgetManager");
			this.collectionLock.EnterReadLock();
			try
			{
				XElement xelement2 = new XElement("CurrentDispatched");
				foreach (WorkType workType in this.dispatchedEntriesPerWorkType.Keys)
				{
					XElement xelement3 = new XElement("WorkType");
					xelement3.Add(new XElement("workType", workType.ToString()));
					XElement content = new XElement("count", this.dispatchedEntriesPerWorkType[workType]);
					xelement3.Add(content);
					xelement2.Add(xelement3);
				}
				xelement.Add(xelement2);
				XElement xelement4 = new XElement("RollingAverageDispatched");
				long num = 0L;
				foreach (WorkType workType2 in this.rollingAverageDispatchedEntries.Keys)
				{
					XElement xelement5 = new XElement("WorkType");
					xelement5.Add(new XElement("workType", workType2.ToString()));
					XElement content2 = new XElement("count", this.rollingAverageDispatchedEntries[workType2].CalculateAverageAcrossAllSamples(out num));
					xelement5.Add(content2);
					xelement4.Add(xelement5);
				}
				xelement.Add(xelement4);
				XElement xelement6 = new XElement("BudgetUsed");
				foreach (WorkType workType3 in this.rollingAverageDispatchedEntries.Keys)
				{
					XElement xelement7 = new XElement("WorkType");
					xelement7.Add(new XElement("workType", workType3.ToString()));
					XElement content3 = new XElement("percentOfBudgetUsed", this.GetPercentOfBudgetUsed(workType3).ToString("F"));
					xelement7.Add(content3);
					xelement6.Add(xelement7);
				}
				xelement.Add(xelement6);
			}
			finally
			{
				this.collectionLock.ExitReadLock();
			}
			return xelement;
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.sampleTimer != null)
			{
				this.sampleTimer.Dispose(true);
				this.sampleTimer = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WorkTypeBudgetManager>(this);
		}

		protected void SampleDispatchedWork()
		{
			this.syncLogSession.LogDebugging((TSLID)1322UL, "SampleDispatchedWork", new object[0]);
			this.collectionLock.EnterWriteLock();
			try
			{
				this.cachedTotalSyncTime = 0L;
				WorkType[] array = (WorkType[])Enum.GetValues(typeof(WorkType));
				foreach (WorkType workType in array)
				{
					int countOfDispatchedWork = this.GetCountOfDispatchedWork(workType);
					this.cachedTotalSyncTime += (long)countOfDispatchedWork;
					this.UpdateWorkType(workType, (long)countOfDispatchedWork);
				}
				if (this.lastHealthLogCollectionTime == null || ExDateTime.UtcNow - this.lastHealthLogCollectionTime > this.healthLogDataCollectionFrequency)
				{
					this.lastHealthLogCollectionTime = new ExDateTime?(ExDateTime.UtcNow);
					KeyValuePair<string, object>[] array3 = new KeyValuePair<string, object>[this.rollingAverageDispatchedEntries.Keys.Count * 2];
					int num = 0;
					foreach (WorkType workType2 in this.rollingAverageDispatchedEntries.Keys)
					{
						double percentOfBudgetUsed = this.GetPercentOfBudgetUsed(workType2);
						array3[num++] = new KeyValuePair<string, object>(string.Format("{0}P", workType2.ToString()), percentOfBudgetUsed.ToString("0.0"));
						long num2 = 0L;
						long num3 = this.rollingAverageDispatchedEntries[workType2].CalculateAverageAcrossAllSamples(out num2);
						array3[num++] = new KeyValuePair<string, object>(string.Format("{0}D", workType2.ToString()), num3.ToString());
					}
					this.syncHealthLogger.LogWorkTypeBudgets(array3);
				}
			}
			finally
			{
				this.collectionLock.ExitWriteLock();
				this.sampleTimer.Change((int)this.sampleDispatchedWorkFrequency.TotalMilliseconds, -1);
			}
		}

		private double GetPercentOfBudgetUsed(WorkType workType)
		{
			if (this.cachedTotalSyncTime == 0L)
			{
				return 0.0;
			}
			if (!this.rollingAverageDispatchedEntries.ContainsKey(workType))
			{
				return 0.0;
			}
			WorkTypeDefinition workTypeDefinition = WorkTypeManager.Instance.GetWorkTypeDefinition(workType);
			long num = 0L;
			long num2 = 100L * this.rollingAverageDispatchedEntries[workType].CalculateAverageAcrossAllSamples(out num) / this.cachedTotalSyncTime;
			return 100.0 * ((double)num2 / (double)workTypeDefinition.Weight);
		}

		private void SampleDispatchedWork(object state)
		{
			this.SampleDispatchedWork();
		}

		private void UpdateWorkType(WorkType workType, long countOfDispatchedWork)
		{
			if (!this.rollingAverageDispatchedEntries.ContainsKey(workType))
			{
				SlidingAverageCounter value = new SlidingAverageCounter(this.slidingWindowLength, this.slidingBucketLength);
				this.rollingAverageDispatchedEntries.Add(workType, value);
			}
			this.rollingAverageDispatchedEntries[workType].AddValue(countOfDispatchedWork);
		}

		private int GetCountOfDispatchedWork(WorkType workType)
		{
			int result;
			if (!this.dispatchedEntriesPerWorkType.TryGetValue(workType, out result))
			{
				return 0;
			}
			return result;
		}

		private readonly TimeSpan healthLogDataCollectionFrequency = TimeSpan.FromMinutes(1.0);

		private readonly Dictionary<WorkType, int> dispatchedEntriesPerWorkType;

		private readonly Dictionary<WorkType, SlidingAverageCounter> rollingAverageDispatchedEntries;

		private readonly TimeSpan slidingWindowLength;

		private readonly TimeSpan slidingBucketLength;

		private readonly TimeSpan sampleDispatchedWorkFrequency;

		private readonly ReaderWriterLockSlim collectionLock = new ReaderWriterLockSlim();

		private readonly SyncLogSession syncLogSession;

		private long cachedTotalSyncTime;

		private GuardedTimer sampleTimer;

		private ExDateTime? lastHealthLogCollectionTime = null;

		private ISyncHealthLog syncHealthLogger;
	}
}
