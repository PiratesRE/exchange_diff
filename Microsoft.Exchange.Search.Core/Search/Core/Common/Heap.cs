using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal class Heap
	{
		internal Heap(IComparer<IHeapItem> weightComparer)
		{
			this.data = new List<IHeapItem>();
			this.weightComparer = weightComparer;
		}

		internal bool IsEmpty
		{
			get
			{
				return this.data.Count == 0;
			}
		}

		internal int Count
		{
			get
			{
				return this.data.Count;
			}
		}

		public override string ToString()
		{
			if (this.IsEmpty)
			{
				return "(empty)";
			}
			StringBuilder stringBuilder = new StringBuilder("root-> ");
			bool flag = true;
			foreach (IHeapItem value in this.data)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(value);
				flag = false;
			}
			return stringBuilder.ToString();
		}

		internal bool Contains(IHeapItem item)
		{
			return item.Handle >= 0 && item.Handle < this.data.Count;
		}

		internal void Update(IHeapItem item)
		{
			if (item.Handle > this.data.Count - 1 || item.Handle < 0)
			{
				throw new ArgumentException(string.Format("heapHandle, invalid value: {0}, current size: {1}", item.Handle, this.data.Count));
			}
			this.data[item.Handle] = item;
			if (!this.AdjustUp(item))
			{
				this.AdjustDown(item);
			}
		}

		internal void Push(IHeapItem item)
		{
			this.data.Add(item);
			item.Handle = this.data.Count - 1;
			this.AdjustUp(item);
		}

		internal bool TryPop(out IHeapItem item)
		{
			item = null;
			if (this.data.Count == 0)
			{
				return false;
			}
			item = this.data[0];
			this.data[0] = this.data[this.data.Count - 1];
			this.data[0].Handle = 0;
			this.data.RemoveAt(this.data.Count - 1);
			if (this.Count > 0)
			{
				this.AdjustDown(this.data[0]);
			}
			return true;
		}

		internal void Flush()
		{
			this.data.Clear();
		}

		private void Swap(IHeapItem left, IHeapItem right)
		{
			IHeapItem value = this.data[left.Handle];
			this.data[left.Handle] = this.data[right.Handle];
			this.data[right.Handle] = value;
			int handle = left.Handle;
			left.Handle = right.Handle;
			right.Handle = handle;
		}

		private IHeapItem GetLeft(IHeapItem item)
		{
			int num = item.Handle * 2 + 1;
			if (num >= this.data.Count)
			{
				return null;
			}
			return this.data[num];
		}

		private IHeapItem GetRight(IHeapItem item)
		{
			IHeapItem left = this.GetLeft(item);
			if (left == null)
			{
				return null;
			}
			int num = left.Handle + 1;
			if (num >= this.data.Count)
			{
				return null;
			}
			return this.data[num];
		}

		private IHeapItem GetParent(IHeapItem item)
		{
			if (item.Handle <= 0)
			{
				return null;
			}
			return this.data[(item.Handle + 1) / 2 - 1];
		}

		private bool AdjustUp(IHeapItem item)
		{
			bool result = false;
			IHeapItem parent = this.GetParent(item);
			while (parent != null && this.weightComparer.Compare(parent, item) < 0)
			{
				this.Swap(parent, item);
				parent = this.GetParent(item);
				result = true;
			}
			return result;
		}

		private bool AdjustDown(IHeapItem item)
		{
			bool result = false;
			IHeapItem left = this.GetLeft(item);
			IHeapItem right = this.GetRight(item);
			while (left != null || right != null)
			{
				IHeapItem heapItem = item;
				if (left != null && this.weightComparer.Compare(left, heapItem) > 0)
				{
					heapItem = left;
				}
				if (right != null && this.weightComparer.Compare(right, heapItem) > 0)
				{
					heapItem = right;
				}
				if (heapItem == item)
				{
					break;
				}
				this.Swap(item, heapItem);
				result = true;
				left = this.GetLeft(item);
				right = this.GetRight(item);
			}
			return result;
		}

		private List<IHeapItem> data;

		private IComparer<IHeapItem> weightComparer;
	}
}
