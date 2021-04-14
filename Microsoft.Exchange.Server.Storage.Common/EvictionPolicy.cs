using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods.Linq;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public abstract class EvictionPolicy<TKey>
	{
		public EvictionPolicy(int capacity)
		{
			this.capacity = capacity;
			this.keysToCleanup = new HashSet<TKey>();
		}

		public int Capacity
		{
			get
			{
				return this.capacity;
			}
		}

		public virtual int Count
		{
			get
			{
				return this.keysToCleanup.Count;
			}
		}

		public abstract void EvictionCheckpoint();

		public abstract void Insert(TKey key);

		public abstract void Remove(TKey key);

		public abstract void KeyAccess(TKey key);

		public virtual bool Contains(TKey key)
		{
			return this.ContainsKeyToCleanup(key);
		}

		public virtual void Reset()
		{
			this.keysToCleanup.Clear();
		}

		public virtual TKey[] GetKeysToCleanup(bool clear)
		{
			TKey[] result = Array<TKey>.Empty;
			if (this.keysToCleanup.Count != 0)
			{
				result = this.keysToCleanup.ToArray<TKey>();
				if (clear)
				{
					this.keysToCleanup.Clear();
				}
			}
			return result;
		}

		public int CountOfKeysToCleanup
		{
			get
			{
				return this.keysToCleanup.Count;
			}
		}

		public void AddKeyToCleanup(TKey key)
		{
			this.keysToCleanup.Add(key);
		}

		public void RemoveKeyToCleanup(TKey key)
		{
			this.keysToCleanup.Remove(key);
		}

		public bool ContainsKeyToCleanup(TKey key)
		{
			return this.keysToCleanup.Contains(key);
		}

		private readonly int capacity;

		private HashSet<TKey> keysToCleanup;
	}
}
