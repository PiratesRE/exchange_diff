using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal class FifoDictionaryCache<TKey, TObject> where TKey : class where TObject : class
	{
		public FifoDictionaryCache(int maximumSize = 10000, IEqualityComparer<TKey> comparer = null) : this(maximumSize, null, null, comparer)
		{
		}

		protected FifoDictionaryCache(int maximumSize = 10000, Dictionary<TKey, TObject> hashSet = null, Queue<TKey> creationOrder = null, IEqualityComparer<TKey> comparer = null)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("maximumSize", maximumSize, 1, int.MaxValue);
			this.maxNumberOfElements = maximumSize;
			this.existingInstances = (hashSet ?? new Dictionary<TKey, TObject>(comparer));
			this.creationOrder = (creationOrder ?? new Queue<TKey>(maximumSize));
		}

		public virtual bool Add(TKey key, TObject property)
		{
			if (this.existingInstances.ContainsKey(key))
			{
				return false;
			}
			bool result = false;
			if (this.creationOrder.Count >= this.maxNumberOfElements)
			{
				TKey key2 = this.creationOrder.Dequeue();
				this.existingInstances.Remove(key2);
				result = true;
			}
			this.existingInstances.Add(key, property);
			this.creationOrder.Enqueue(key);
			return result;
		}

		public virtual bool ContainsKey(TKey key)
		{
			return this.existingInstances.ContainsKey(key);
		}

		public virtual bool TryGetValue(TKey key, out TObject property)
		{
			return this.existingInstances.TryGetValue(key, out property);
		}

		public const int DefaultMaximumSize = 10000;

		private readonly Dictionary<TKey, TObject> existingInstances;

		private readonly Queue<TKey> creationOrder;

		private readonly int maxNumberOfElements;
	}
}
