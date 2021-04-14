using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	public class CounterDictionary<T>
	{
		public CounterDictionary() : this(0, int.MaxValue)
		{
		}

		public CounterDictionary(int capacity) : this(capacity, int.MaxValue)
		{
		}

		public CounterDictionary(int capacity, int maxSize)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (maxSize < 0)
			{
				throw new ArgumentOutOfRangeException("maxSize");
			}
			this.dictionary = new Dictionary<T, int>(capacity);
			this.maxSize = maxSize;
		}

		public int this[T key]
		{
			get
			{
				int result;
				lock (this.dictionary)
				{
					int num = 0;
					this.dictionary.TryGetValue(key, out num);
					result = num;
				}
				return result;
			}
		}

		public int IncrementCounter(T key)
		{
			return this.AddCounter(key, 1);
		}

		public int DecrementCounter(T key)
		{
			return this.AddCounter(key, -1);
		}

		private int AddCounter(T key, int delta)
		{
			int result;
			lock (this.dictionary)
			{
				int num = 0;
				bool flag2 = this.dictionary.TryGetValue(key, out num);
				int num2 = num + delta;
				if (flag2 || this.dictionary.Count < this.maxSize)
				{
					this.dictionary[key] = num2;
					result = num2;
				}
				else
				{
					result = num;
				}
			}
			return result;
		}

		private Dictionary<T, int> dictionary;

		private int maxSize;
	}
}
