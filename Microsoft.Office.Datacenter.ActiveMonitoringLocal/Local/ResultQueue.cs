using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring.Local
{
	internal class ResultQueue<TWorkItemResult> where TWorkItemResult : WorkItemResult
	{
		internal ResultQueue(int size)
		{
			this.data = new ResultQueue<TWorkItemResult>.Cell[size];
			this.index = -1;
			this.lastUsedId = 0;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.lastUsedId == 0;
			}
		}

		internal bool Add(TWorkItemResult item)
		{
			if (item == null)
			{
				throw new ArgumentException("item");
			}
			this.GetNewest();
			bool flag = false;
			this.spinLock.Enter(ref flag);
			bool isEmpty = this.IsEmpty;
			int num = this.index + 1;
			if (num == this.data.Length)
			{
				num = 0;
			}
			this.lastUsedId++;
			this.data[num] = default(ResultQueue<TWorkItemResult>.Cell);
			this.data[num].Item = item;
			this.data[num].Id = (long)this.lastUsedId;
			this.index = num;
			this.spinLock.Exit();
			return isEmpty;
		}

		internal TWorkItemResult GetNewest()
		{
			if (this.IsEmpty)
			{
				return default(TWorkItemResult);
			}
			return this.data[this.index].Item;
		}

		internal IEnumerable<TWorkItemResult> GetItems(DateTime startTime)
		{
			int currentIndex = this.index;
			if (!this.IsEmpty)
			{
				long previousId = long.MaxValue;
				TWorkItemResult currentItem = this.data[currentIndex].Item;
				long currentId = this.data[currentIndex].Id;
				while (currentId < previousId && currentId > 0L)
				{
					if (currentItem.ExecutionEndTime >= startTime)
					{
						yield return currentItem;
					}
					previousId = currentId;
					currentIndex--;
					if (currentIndex < 0)
					{
						currentIndex = this.data.Length - 1;
					}
					currentItem = this.data[currentIndex].Item;
					currentId = this.data[currentIndex].Id;
				}
			}
			yield break;
		}

		private ResultQueue<TWorkItemResult>.Cell[] data;

		private int index;

		private int lastUsedId;

		private SpinLock spinLock;

		private struct Cell
		{
			public long Id;

			public TWorkItemResult Item;
		}
	}
}
