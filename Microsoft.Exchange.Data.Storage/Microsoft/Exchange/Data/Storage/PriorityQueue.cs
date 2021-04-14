using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PriorityQueue<T> where T : IComparable<T>
	{
		public PriorityQueue()
		{
			this.data = new List<T>();
		}

		public void Enqueue(T item)
		{
			this.data.Add(item);
			int num;
			for (int i = this.data.Count - 1; i > 0; i = num)
			{
				num = (i - 1) / 2;
				T t = this.data[i];
				if (t.CompareTo(this.data[num]) >= 0)
				{
					return;
				}
				T value = this.data[i];
				this.data[i] = this.data[num];
				this.data[num] = value;
			}
		}

		public T Dequeue()
		{
			int num = this.data.Count - 1;
			T result = this.data[0];
			this.data[0] = this.data[num];
			this.data.RemoveAt(num);
			num--;
			int num2 = 0;
			for (;;)
			{
				int num3 = num2 * 2 + 1;
				if (num3 > num)
				{
					break;
				}
				int num4 = num3 + 1;
				if (num4 <= num)
				{
					T t = this.data[num4];
					if (t.CompareTo(this.data[num3]) < 0)
					{
						num3 = num4;
					}
				}
				T t2 = this.data[num2];
				if (t2.CompareTo(this.data[num3]) <= 0)
				{
					break;
				}
				T value = this.data[num2];
				this.data[num2] = this.data[num3];
				this.data[num3] = value;
				num2 = num3;
			}
			return result;
		}

		public T Peek()
		{
			return this.data[0];
		}

		public int Count()
		{
			return this.data.Count;
		}

		public override string ToString()
		{
			string arg = this.data.Aggregate(string.Empty, (string current, T t) => current + t.ToString() + " ");
			return arg + "count = " + this.data.Count;
		}

		public bool IsConsistent()
		{
			if (this.data.Count == 0)
			{
				return true;
			}
			int num = this.data.Count - 1;
			for (int i = 0; i < this.data.Count; i++)
			{
				int num2 = 2 * i + 1;
				int num3 = 2 * i + 2;
				if (num2 <= num)
				{
					T t = this.data[i];
					if (t.CompareTo(this.data[num2]) > 0)
					{
						return false;
					}
				}
				if (num3 <= num)
				{
					T t2 = this.data[i];
					if (t2.CompareTo(this.data[num3]) > 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private readonly List<T> data;
	}
}
