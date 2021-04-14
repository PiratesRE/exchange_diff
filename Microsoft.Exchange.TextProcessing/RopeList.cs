using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextProcessing
{
	public class RopeList<T>
	{
		public RopeList(int contiguousCapacity = 1024)
		{
			this.contiguousCapacity = contiguousCapacity;
			this.items.Add(new T[this.contiguousCapacity]);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public T this[int index]
		{
			get
			{
				return this.items[index / this.contiguousCapacity][index % this.contiguousCapacity];
			}
			set
			{
				this.items[index / this.contiguousCapacity][index % this.contiguousCapacity] = value;
			}
		}

		public void Add(T item)
		{
			if ((this.count + 1) / this.contiguousCapacity >= this.items.Count)
			{
				this.items.Add(new T[this.contiguousCapacity]);
			}
			this[this.count] = item;
			this.count++;
		}

		private readonly int contiguousCapacity;

		private List<T[]> items = new List<T[]>();

		private int count;
	}
}
