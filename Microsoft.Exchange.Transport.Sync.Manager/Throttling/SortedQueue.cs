using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SortedQueue<T> : IEnumerable<T>, IEnumerable where T : IComparable<T>
	{
		public SortedQueue() : this(4)
		{
		}

		public SortedQueue(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("Capacity should not be less than zero.");
			}
			this.array = new T[capacity];
			this.head = 0;
			this.tail = 0;
			this.count = 0;
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public bool IsEmpty()
		{
			return this.count == 0;
		}

		public void Clear()
		{
			if (this.head < this.tail)
			{
				Array.Clear(this.array, this.head, this.count);
			}
			else
			{
				Array.Clear(this.array, this.head, this.array.Length - this.head);
				Array.Clear(this.array, 0, this.tail);
			}
			this.head = 0;
			this.tail = 0;
			this.count = 0;
		}

		public T Dequeue()
		{
			T result = this.Peek();
			this.array[this.head] = default(T);
			this.head = (this.head + 1) % this.array.Length;
			this.count--;
			return result;
		}

		public void Enqueue(T item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (this.count == this.array.Length)
			{
				this.GrowArray();
			}
			int num;
			int num2;
			for (num = this.tail; num != this.head; num = num2)
			{
				num2 = (num - 1 + this.array.Length) % this.array.Length;
				if (item.CompareTo(this.array[num2]) >= 0)
				{
					break;
				}
				this.array[num] = this.array[num2];
			}
			this.array[num] = item;
			this.tail = (this.tail + 1) % this.array.Length;
			this.count++;
		}

		public T Peek()
		{
			if (this.IsEmpty())
			{
				throw new InvalidOperationException("SortedQueue is empty");
			}
			return this.array[this.head];
		}

		public void TrimExcess()
		{
			int num = (int)((double)this.array.Length * 0.9);
			if (this.count < num)
			{
				this.SetCapacity(this.count);
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			int currentSlot = this.head;
			for (int counted = 0; counted < this.count; counted++)
			{
				yield return this.array[currentSlot];
				currentSlot = (currentSlot + 1) % this.array.Length;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		private void GrowArray()
		{
			int num = this.array.Length * 2;
			if (num < this.array.Length + 4)
			{
				num = this.array.Length + 4;
			}
			this.SetCapacity(num);
		}

		private void SetCapacity(int capacity)
		{
			T[] destinationArray = new T[capacity];
			if (this.count > 0)
			{
				if (this.head < this.tail)
				{
					Array.Copy(this.array, this.head, destinationArray, 0, this.count);
				}
				else
				{
					Array.Copy(this.array, this.head, destinationArray, 0, this.array.Length - this.head);
					Array.Copy(this.array, 0, destinationArray, this.array.Length - this.head, this.tail);
				}
			}
			this.array = destinationArray;
			this.head = 0;
			this.tail = ((this.count == capacity) ? 0 : this.count);
		}

		private const int DefaultCapacity = 4;

		private const int GrowFactor = 2;

		private const int MinimumGrow = 4;

		private const double ShrinkFactor = 0.9;

		private T[] array;

		private int head;

		private int tail;

		private int count;
	}
}
