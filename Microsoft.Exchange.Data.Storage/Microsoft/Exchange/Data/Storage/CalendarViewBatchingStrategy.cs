using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CalendarViewBatchingStrategy
	{
		private CalendarViewBatchingStrategy(int? idealMaxCount, CalendarViewQueryResumptionPoint queryResumptionPoint)
		{
			this.idealMaxCount = idealMaxCount;
			this.queryResumptionPoint = queryResumptionPoint;
			this.ResetCount();
			this.instanceKeyIndex = -1;
			this.sortKeyIndex = -1;
			this.keyIndicesAreSet = false;
		}

		public static CalendarViewBatchingStrategy CreateNoneBatchingInstance()
		{
			return new CalendarViewBatchingStrategy(null, null);
		}

		public static CalendarViewBatchingStrategy CreateNewBatchingInstance(int idealMaxCount)
		{
			return new CalendarViewBatchingStrategy(new int?(idealMaxCount), CalendarViewBatchingStrategy.CreateResumptionPointWithoutInstanceKey(false));
		}

		public static CalendarViewBatchingStrategy CreateResumingInstance(int idealMaxCount, CalendarViewQueryResumptionPoint resumptionPoint)
		{
			Util.ThrowOnNullArgument(resumptionPoint, "resumptionPoint");
			return new CalendarViewBatchingStrategy(new int?(idealMaxCount), resumptionPoint);
		}

		public bool ShouldBatch
		{
			get
			{
				return this.idealMaxCount != null;
			}
		}

		public bool ShouldQuerySingleInstanceMeetings
		{
			get
			{
				return this.queryResumptionPoint == null || this.queryResumptionPoint.ResumeToSingleInstanceMeetings;
			}
		}

		public bool ReachedBatchSizeLimit
		{
			get
			{
				return this.idealMaxCount != null && this.idealMaxCount.Value <= this.addedItemsCount;
			}
		}

		public CalendarViewQueryResumptionPoint ResumptionPoint
		{
			get
			{
				return this.queryResumptionPoint;
			}
		}

		public void ResetCount()
		{
			this.addedItemsCount = 0;
		}

		public bool TryGetNextBatch(QueryResult result, int storageLimit, int defaultBatchSize, bool recurring, bool isFirstFetch, out object[][] nextBatch, out long getRowsTime)
		{
			getRowsTime = 0L;
			if (this.ShouldBatch && !this.keyIndicesAreSet)
			{
				throw new InvalidOperationException("When batching is required, the critical columns' indices should be set prior to calling this method.");
			}
			int currentBatchSize = this.GetCurrentBatchSize(storageLimit, defaultBatchSize);
			bool flag;
			if (currentBatchSize == 0)
			{
				object[][] rows = result.GetRows(1);
				if (rows.Length > 0)
				{
					if (this.ShouldBatch)
					{
						this.ResetQueryResumptionPoint(rows[0], recurring);
					}
					ExTraceGlobals.StorageTracer.TraceDebug<string, int, bool>((long)this.GetHashCode(), "{0}. Batch size is equal to zero; terminating the process... (items in view: {1}; RecurringPhase: {2}).", "CalendarViewBatchingStrategy::TryGetNextBatch", this.addedItemsCount, recurring);
				}
				else
				{
					if (this.ShouldBatch)
					{
						this.PhaseComplete();
					}
					ExTraceGlobals.StorageTracer.TraceDebug<string, int, bool>((long)this.GetHashCode(), "{0}. Batch size is equal to zero, and there are no more rows to process (items in view: {1}; RecurringPhase: {2}).", "CalendarViewBatchingStrategy::TryGetNextBatch", this.addedItemsCount, recurring);
				}
				nextBatch = null;
				flag = false;
			}
			else if (this.ShouldBatch && isFirstFetch)
			{
				flag = this.TryResumeQueryOrStartFromBeginning(result, currentBatchSize, recurring, out nextBatch);
			}
			else
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "{0}. Getting a batch with the size of {1} (items in view: {2}; RecurringPhase: {3}).", new object[]
				{
					"CalendarViewBatchingStrategy::TryGetNextBatch",
					currentBatchSize,
					this.addedItemsCount,
					recurring
				});
				Stopwatch stopwatch = Stopwatch.StartNew();
				try
				{
					nextBatch = result.GetRows(currentBatchSize, out flag);
				}
				finally
				{
					stopwatch.Stop();
					getRowsTime = stopwatch.ElapsedMilliseconds;
				}
			}
			if (this.ShouldBatch && currentBatchSize != 0 && !flag)
			{
				this.PhaseComplete();
			}
			return flag;
		}

		public void SetColumnIndices(int instanceKeyIndex, int sortKeyIndex)
		{
			this.instanceKeyIndex = instanceKeyIndex;
			this.sortKeyIndex = sortKeyIndex;
			this.keyIndicesAreSet = true;
		}

		public bool ShouldAddNewRow(object[] newRow, bool recurring)
		{
			bool result = true;
			if (this.ShouldBatch && this.addedItemsCount >= this.idealMaxCount)
			{
				result = false;
				ExTraceGlobals.StorageTracer.TraceDebug<string, int, bool>((long)this.GetHashCode(), "{0}. Hit the max count. Terminating the batch and resetting the resumption point (items in view: {1}; RecurringPhase: {2}).", "CalendarViewBatchingStrategy::ShouldAddNewRow", this.addedItemsCount, recurring);
				this.ResetQueryResumptionPoint(newRow, recurring);
			}
			return result;
		}

		public void AddNewRow(IList<object[]> results, object[] newRow)
		{
			this.addedItemsCount++;
			results.Add(newRow);
		}

		private static CalendarViewQueryResumptionPoint CreateResumptionPointWithoutInstanceKey(bool recurring)
		{
			return CalendarViewQueryResumptionPoint.CreateInstance(recurring, null, null);
		}

		private void PhaseComplete()
		{
			this.queryResumptionPoint = CalendarViewBatchingStrategy.CreateResumptionPointWithoutInstanceKey(this.queryResumptionPoint.ResumeToSingleInstanceMeetings);
		}

		private void ResetQueryResumptionPoint(object[] row, bool recurring)
		{
			this.queryResumptionPoint = CalendarViewQueryResumptionPoint.CreateInstance(recurring, row[this.instanceKeyIndex] as byte[], row[this.sortKeyIndex] as ExDateTime?);
		}

		private int GetCurrentBatchSize(int storageLimit, int defaultBatchSize)
		{
			int result;
			if (storageLimit > this.addedItemsCount)
			{
				int val = storageLimit - this.addedItemsCount;
				if (this.ShouldBatch)
				{
					if (this.idealMaxCount.Value > this.addedItemsCount)
					{
						int val2 = this.idealMaxCount.Value - this.addedItemsCount;
						result = Math.Min(defaultBatchSize, Math.Min(val2, val));
					}
					else
					{
						result = 0;
					}
				}
				else
				{
					result = Math.Min(defaultBatchSize, val);
				}
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private bool TryResumeQueryOrStartFromBeginning(QueryResult result, int currentBatchSize, bool recurring, out object[][] nextBatch)
		{
			bool flag = this.queryResumptionPoint.TryResume(result, this.sortKeyIndex, SeekReference.OriginBeginning, currentBatchSize, out nextBatch);
			ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "{0}. Resumption to the previous point was {1} (Batch Size: {2}; Items Already in View: {3}; RecurringPhase: {4}).", new object[]
			{
				"CalendarViewBatchingStrategy::TryGetNextBatch",
				flag ? "Successful" : "Unsuccessful",
				currentBatchSize,
				this.addedItemsCount,
				recurring
			});
			if (!flag)
			{
				ExTraceGlobals.StorageTracer.TraceDebug((long)this.GetHashCode(), "{0}. Starting from scratch and getting a batch with the size of {1} (items in view: {2}; RecurringPhase: {3}).", new object[]
				{
					"CalendarViewBatchingStrategy::TryGetNextBatch",
					currentBatchSize,
					this.addedItemsCount,
					recurring
				});
				result.SeekToOffset(SeekReference.OriginBeginning, 0);
				nextBatch = result.GetRows(currentBatchSize, out flag);
			}
			return flag;
		}

		private readonly int? idealMaxCount;

		private int addedItemsCount;

		private CalendarViewQueryResumptionPoint queryResumptionPoint;

		private int instanceKeyIndex;

		private int sortKeyIndex;

		private bool keyIndicesAreSet;
	}
}
