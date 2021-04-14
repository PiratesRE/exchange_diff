using System;

namespace Microsoft.Exchange.Provisioning.LoadBalancing
{
	internal class ComparableEntry<T> : IComparable
	{
		public ComparableEntry(T entry)
		{
			this.entry = entry;
			this.count = 0;
		}

		public T Entry
		{
			get
			{
				return this.entry;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
			}
		}

		public int CompareTo(object obj)
		{
			if (!base.GetType().IsInstanceOfType(obj))
			{
				throw new ArgumentException("obj is not of the required type");
			}
			ComparableEntry<T> comparableEntry = (ComparableEntry<T>)obj;
			return this.count.CompareTo(comparableEntry.Count);
		}

		private T entry;

		private int count;
	}
}
