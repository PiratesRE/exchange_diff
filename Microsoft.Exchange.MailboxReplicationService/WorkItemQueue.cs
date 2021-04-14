using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WorkItemQueue
	{
		public WorkItemQueue()
		{
			this.workItems = new List<WorkItem>(10);
			this.periodicWorkItems = new List<WorkItem>(1);
		}

		public void Add(WorkItem workItem)
		{
			if (workItem is IPeriodicWorkItem)
			{
				this.periodicWorkItems.Add(workItem);
				return;
			}
			this.workItems.Add(workItem);
		}

		public ExDateTime ScheduledRunTime
		{
			get
			{
				ExDateTime exDateTime = (this.workItems.Count == 0) ? ExDateTime.MaxValue : this.workItems[0].ScheduledRunTime;
				foreach (WorkItem workItem in this.periodicWorkItems)
				{
					if (workItem.ScheduledRunTime < exDateTime)
					{
						exDateTime = workItem.ScheduledRunTime;
					}
				}
				return exDateTime;
			}
			set
			{
				if (this.workItems.Count > 0)
				{
					this.workItems[0].ScheduledRunTime = value;
					return;
				}
				this.periodicWorkItems[0].ScheduledRunTime = value;
			}
		}

		public void Clear()
		{
			this.workItems.Clear();
			this.periodicWorkItems.Clear();
		}

		public void Remove(WorkItem workItem)
		{
			if (workItem is IPeriodicWorkItem)
			{
				workItem.ScheduledRunTime = ExDateTime.UtcNow.Add(((IPeriodicWorkItem)workItem).PeriodicInterval);
				return;
			}
			this.workItems.Remove(workItem);
		}

		public bool IsEmpty()
		{
			return this.workItems.Count == 0;
		}

		public IEnumerable<WorkItem> GetCandidateWorkItems()
		{
			foreach (WorkItem workItem in this.periodicWorkItems)
			{
				yield return workItem;
			}
			if (!this.IsEmpty())
			{
				yield return this.workItems[0];
			}
			yield break;
		}

		private readonly List<WorkItem> workItems;

		private readonly List<WorkItem> periodicWorkItems;
	}
}
