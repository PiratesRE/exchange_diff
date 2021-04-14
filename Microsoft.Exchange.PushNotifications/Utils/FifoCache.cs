using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Utils
{
	internal class FifoCache<TObject> where TObject : class
	{
		public FifoCache(int maximumSize = 10000, IEqualityComparer<TObject> comparer = null) : this(maximumSize, null, null, comparer)
		{
		}

		protected FifoCache(int maximumSize = 10000, HashSet<TObject> hashSet = null, Queue<TObject> creationOrder = null, IEqualityComparer<TObject> comparer = null)
		{
			ArgumentValidator.ThrowIfOutOfRange<int>("maximumSize", maximumSize, 1, int.MaxValue);
			this.maxNumberOfElements = maximumSize;
			this.existingInstances = (hashSet ?? new HashSet<TObject>(comparer));
			this.creationOrder = (creationOrder ?? new Queue<TObject>(maximumSize));
		}

		public bool Add(TObject property)
		{
			bool result = false;
			if (this.creationOrder.Count >= this.maxNumberOfElements && !this.existingInstances.Contains(property))
			{
				TObject item = this.creationOrder.Dequeue();
				this.existingInstances.Remove(item);
				result = true;
			}
			if (this.existingInstances.Add(property))
			{
				this.creationOrder.Enqueue(property);
			}
			return result;
		}

		public bool Contains(TObject property)
		{
			return this.existingInstances.Contains(property);
		}

		public const int DefaultMaximumSize = 10000;

		private readonly HashSet<TObject> existingInstances;

		private readonly Queue<TObject> creationOrder;

		private readonly int maxNumberOfElements;
	}
}
