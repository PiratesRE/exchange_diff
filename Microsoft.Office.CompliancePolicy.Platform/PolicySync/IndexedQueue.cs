using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	internal sealed class IndexedQueue
	{
		public bool TryGetValue(Guid key, out WorkItemBase value)
		{
			value = null;
			DateTime key2;
			return this.index.TryGetValue(key, out key2) && this.sortedList.TryGetValue(key2, out value);
		}

		public void Enqueue(WorkItemBase item)
		{
			ArgumentValidator.ThrowIfNull("item", item);
			Guid primaryKey = item.GetPrimaryKey();
			DateTime dateTime = this.AdjustExecuteTimeIfNecessary(item.ExecuteTimeUTC, item.ProcessNow);
			item.ExecuteTimeUTC = dateTime;
			if (!this.index.ContainsKey(primaryKey))
			{
				this.sortedList.Add(dateTime, item);
				this.index.Add(primaryKey, dateTime);
				return;
			}
			if (this.index[primaryKey] != dateTime)
			{
				this.sortedList.Remove(this.index[primaryKey]);
				this.sortedList.Add(dateTime, item);
				this.index[primaryKey] = dateTime;
				return;
			}
			this.sortedList[dateTime] = item;
		}

		public IList<WorkItemBase> Dequeue(int maxCount)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("maxCount", maxCount);
			List<WorkItemBase> list = new List<WorkItemBase>();
			DateTime utcNow = DateTime.UtcNow;
			while (list.Count < maxCount && !this.IsEmpty() && this.sortedList.Keys[0] <= utcNow)
			{
				WorkItemBase workItemBase = this.sortedList.Values[0];
				this.index.Remove(workItemBase.GetPrimaryKey());
				this.sortedList.RemoveAt(0);
				list.Add(workItemBase);
			}
			if (!list.Any<WorkItemBase>())
			{
				return null;
			}
			return list;
		}

		public bool IsEmpty()
		{
			return !this.index.Any<KeyValuePair<Guid, DateTime>>();
		}

		public int Count()
		{
			return this.index.Count;
		}

		private DateTime AdjustExecuteTimeIfNecessary(DateTime originalTime, bool insertToHead)
		{
			DateTime dateTime = originalTime;
			if (this.sortedList.Any<KeyValuePair<DateTime, WorkItemBase>>())
			{
				if (insertToHead)
				{
					if (dateTime >= this.sortedList.Keys[0])
					{
						dateTime = this.sortedList.Keys[0] - new TimeSpan(0, 0, 1);
					}
				}
				else
				{
					while (this.sortedList.ContainsKey(dateTime))
					{
						dateTime += new TimeSpan(1L);
					}
				}
			}
			return dateTime;
		}

		private readonly Dictionary<Guid, DateTime> index = new Dictionary<Guid, DateTime>();

		private readonly SortedList<DateTime, WorkItemBase> sortedList = new SortedList<DateTime, WorkItemBase>();
	}
}
