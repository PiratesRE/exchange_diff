using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Collections
{
	public class LRUCache<TKey, TValue> where TKey : IComparable, IComparable<TKey>, IEquatable<TKey>
	{
		public LRUCache(int capacity, Func<TKey, TValue> loadItem) : this(capacity, loadItem, null, null, null, null, null, null)
		{
		}

		public LRUCache(int capacity, Func<TKey, TValue> loadItem, double? maxRatio, Action getCounter, Action missCounter, Func<bool> forceEvictCondition, Func<bool> forceEvictPredicate, Action<TValue> elementEvictCallback)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (loadItem == null)
			{
				throw new ArgumentNullException("loadItem");
			}
			this.capacity = capacity;
			this.loadItem = loadItem;
			this.mapping = new Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>>(capacity);
			this.evictionList = new LinkedList<KeyValuePair<TKey, TValue>>();
			if (maxRatio != null)
			{
				double? num = maxRatio;
				if (num.GetValueOrDefault() > 0.0 || num == null)
				{
					double? num2 = maxRatio;
					if (num2.GetValueOrDefault() < 1.0 || num2 == null)
					{
						goto IL_9E;
					}
				}
				throw new ArgumentException("maxRatio has to be null or (0,1)");
			}
			IL_9E:
			this.maxRatio = maxRatio;
			this.getCounter = (getCounter ?? LRUCache<TKey, TValue>.ActionDefault);
			this.missCounter = (missCounter ?? LRUCache<TKey, TValue>.ActionDefault);
			this.forceEvictCondition = (forceEvictCondition ?? LRUCache<TKey, TValue>.PredicateDefault);
			this.forceEvictPredicate = (forceEvictPredicate ?? LRUCache<TKey, TValue>.PredicateDefault);
			this.elementEvictCallback = (elementEvictCallback ?? LRUCache<TKey, TValue>.ActionTValueDefault);
			this.lockObject = new ReaderWriterLockSlim();
		}

		public void Reset()
		{
			try
			{
				this.lockObject.EnterWriteLock();
				if (this.elementEvictCallback != LRUCache<TKey, TValue>.ActionTValueDefault)
				{
					foreach (KeyValuePair<TKey, TValue> keyValuePair in this.evictionList)
					{
						this.elementEvictCallback(keyValuePair.Value);
					}
				}
				this.mapping.Clear();
				this.evictionList.Clear();
			}
			finally
			{
				try
				{
					this.lockObject.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public void UpdateCapacity(int capacity, Action action)
		{
			try
			{
				this.lockObject.EnterWriteLock();
				this.capacity = capacity;
				if (action != null)
				{
					action();
				}
			}
			finally
			{
				try
				{
					this.lockObject.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		public TValue Get(TKey key)
		{
			bool flag;
			return this.Get(key, out flag);
		}

		public TValue Get(TKey key, out bool elementEvicted)
		{
			elementEvicted = false;
			this.getCounter();
			TValue result;
			try
			{
				this.lockObject.EnterWriteLock();
				TValue tvalue;
				if (this.TryLoadFromCache(key, out tvalue))
				{
					result = tvalue;
				}
				else
				{
					this.missCounter();
					TValue value = this.loadItem(key);
					result = this.AddNewItem(key, value, ref elementEvicted);
				}
			}
			finally
			{
				try
				{
					this.lockObject.ExitWriteLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		protected virtual bool TryLoadFromCache(TKey key, out TValue value)
		{
			LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
			if (this.mapping.TryGetValue(key, out linkedListNode))
			{
				this.evictionList.Remove(linkedListNode);
				this.evictionList.AddFirst(linkedListNode);
				value = linkedListNode.Value.Value;
				return true;
			}
			value = default(TValue);
			return false;
		}

		protected virtual TValue AddNewItem(TKey key, TValue value, ref bool elementEvicted)
		{
			if (this.evictionList.Count >= this.capacity || this.forceEvictCondition())
			{
				double num = (double)this.capacity;
				double? num2 = this.maxRatio;
				int num3 = (int)(num * ((num2 != null) ? num2.GetValueOrDefault() : 1.0));
				while (this.evictionList.Count > 0 && (this.evictionList.Count >= num3 || this.forceEvictPredicate()))
				{
					this.elementEvictCallback(this.evictionList.Last.Value.Value);
					this.mapping.Remove(this.evictionList.Last.Value.Key);
					this.evictionList.RemoveLast();
					elementEvicted = true;
				}
			}
			KeyValuePair<TKey, TValue> value2 = new KeyValuePair<TKey, TValue>(key, value);
			LinkedListNode<KeyValuePair<TKey, TValue>> linkedListNode;
			if (this.mapping.TryGetValue(key, out linkedListNode))
			{
				linkedListNode.Value = value2;
				this.evictionList.Remove(linkedListNode);
			}
			else
			{
				linkedListNode = new LinkedListNode<KeyValuePair<TKey, TValue>>(value2);
				this.mapping.Add(linkedListNode.Value.Key, linkedListNode);
			}
			this.evictionList.AddFirst(linkedListNode);
			return value;
		}

		private static readonly Action ActionDefault = delegate()
		{
		};

		private static readonly Func<bool> PredicateDefault = () => false;

		private static readonly Action<TValue> ActionTValueDefault = delegate(TValue v)
		{
		};

		private readonly Func<TKey, TValue> loadItem;

		private readonly Dictionary<TKey, LinkedListNode<KeyValuePair<TKey, TValue>>> mapping;

		private readonly LinkedList<KeyValuePair<TKey, TValue>> evictionList;

		private readonly double? maxRatio;

		private readonly Action getCounter;

		private readonly Action missCounter;

		private readonly Func<bool> forceEvictCondition;

		private readonly Func<bool> forceEvictPredicate;

		private readonly Action<TValue> elementEvictCallback;

		private readonly ReaderWriterLockSlim lockObject;

		private int capacity;
	}
}
