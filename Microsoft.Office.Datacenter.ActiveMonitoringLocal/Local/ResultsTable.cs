using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal class ResultsTable<TWorkItemResult> : SimpleTable<TWorkItemResult, int, ResultQueue<TWorkItemResult>> where TWorkItemResult : WorkItemResult
	{
		public ResultsTable(int resultHistorySize, bool requiresNameIndex) : base(WorkItemResultIndex<TWorkItemResult>.WorkItemIdAndExecutionEndTime(0, DateTime.MinValue))
		{
			this.resultHistorySize = resultHistorySize;
			if (requiresNameIndex)
			{
				IIndexDescriptor<TWorkItemResult, string> indexDescriptor = WorkItemResultIndex<TWorkItemResult>.ResultNameAndExecutionEndTime(string.Empty, DateTime.MinValue);
				base.AddIndex<string>(indexDescriptor);
			}
		}

		public Action<TWorkItemResult> OnInsertNotificationDelegate { get; set; }

		protected override ResultQueue<TWorkItemResult> CreateSegment(TWorkItemResult item)
		{
			return new ResultQueue<TWorkItemResult>(this.resultHistorySize);
		}

		protected override bool AddToSegment(ResultQueue<TWorkItemResult> segment, TWorkItemResult item)
		{
			TWorkItemResult newest = segment.GetNewest();
			if (newest != null && newest.Exception == item.Exception)
			{
				item.Exception = newest.Exception;
			}
			bool result = segment.Add(item);
			if (this.OnInsertNotificationDelegate != null)
			{
				this.OnInsertNotificationDelegate(item);
			}
			return result;
		}

		protected override IEnumerable<TWorkItemResult> GetItemsFromSegment<TKey>(ResultQueue<TWorkItemResult> segment, IIndexDescriptor<TWorkItemResult, TKey> indexDescriptor)
		{
			DateTime minExecutionEndTime = ((WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexBase<TWorkItemResult, TKey>)indexDescriptor).MinExecutionEndTime;
			return segment.GetItems(minExecutionEndTime);
		}

		protected override IEnumerable<TWorkItemResult> GetItemsFromSegments<TKey>(IEnumerable<ResultQueue<TWorkItemResult>> segments, IIndexDescriptor<TWorkItemResult, TKey> indexDescriptor)
		{
			DateTime minExecutionEndTime = ((WorkItemResultIndex<TWorkItemResult>.WorkItemResultIndexBase<TWorkItemResult, TKey>)indexDescriptor).MinExecutionEndTime;
			return segments.SelectMany((ResultQueue<TWorkItemResult> segment) => segment.GetItems(minExecutionEndTime));
		}

		private readonly int resultHistorySize;
	}
}
