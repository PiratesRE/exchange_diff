using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.TextProcessing
{
	internal class LowAllocSet
	{
		internal LowAllocSet(int initialCapacity = 1024)
		{
			this.primaryTable = new List<long>(initialCapacity);
			for (int i = 0; i < initialCapacity; i++)
			{
				this.primaryTable.Add(long.MaxValue);
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public IEnumerable<long> Values
		{
			get
			{
				if (this.secondaryTable != null)
				{
					return (from value in this.primaryTable
					where value != long.MaxValue
					select value).Concat(this.secondaryTable.Keys);
				}
				return from value in this.primaryTable
				where value != long.MaxValue
				select value;
			}
		}

		public void Add(long value)
		{
			int hash = this.GetHash(value);
			if (this.primaryTable[hash] == 9223372036854775807L)
			{
				this.primaryTable[hash] = value;
				this.count++;
				return;
			}
			if (this.primaryTable[hash] != value && (this.secondaryTable == null || !this.secondaryTable.ContainsKey(value)))
			{
				if (this.secondaryTable == null)
				{
					this.secondaryTable = new Dictionary<long, bool>(64);
				}
				this.secondaryTable.Add(value, true);
				this.count++;
			}
		}

		public bool Contains(long value)
		{
			int hash = this.GetHash(value);
			return this.primaryTable[hash] != long.MaxValue && (this.primaryTable[hash] == value || (this.secondaryTable != null && this.secondaryTable.ContainsKey(value)));
		}

		private int GetHash(long value)
		{
			return Math.Abs((int)value ^ (int)(value >> 32)) % this.primaryTable.Count;
		}

		private const long EmptyCellValue = 9223372036854775807L;

		private Dictionary<long, bool> secondaryTable;

		private List<long> primaryTable;

		private int count;
	}
}
