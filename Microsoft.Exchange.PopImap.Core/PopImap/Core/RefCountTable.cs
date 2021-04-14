using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.PopImap.Core
{
	internal class RefCountTable<TKey>
	{
		internal RefCountTable()
		{
			this.refCounters = new Dictionary<TKey, int>();
		}

		public Dictionary<TKey, int> Counters
		{
			get
			{
				return this.refCounters;
			}
		}

		public int Add(TKey key)
		{
			int result;
			lock (((ICollection)this.refCounters).SyncRoot)
			{
				int num;
				if (this.refCounters.TryGetValue(key, out num))
				{
					num = (this.refCounters[key] = num + 1);
					result = num;
				}
				else
				{
					num = 1;
					this.refCounters.Add(key, num);
					result = num;
				}
			}
			return result;
		}

		public void Remove(TKey key)
		{
			lock (((ICollection)this.refCounters).SyncRoot)
			{
				int num;
				if (this.refCounters.TryGetValue(key, out num))
				{
					if (num > 0)
					{
						Dictionary<TKey, int> dictionary;
						(dictionary = this.refCounters)[key] = dictionary[key] - 1;
					}
					else
					{
						this.refCounters.Remove(key);
					}
				}
			}
		}

		public void Remove(TKey key, int number)
		{
			lock (((ICollection)this.refCounters).SyncRoot)
			{
				int num;
				if (this.refCounters.TryGetValue(key, out num))
				{
					if (num > number)
					{
						this.refCounters[key] = num - number;
					}
					else
					{
						this.refCounters.Remove(key);
					}
				}
			}
		}

		private Dictionary<TKey, int> refCounters;
	}
}
