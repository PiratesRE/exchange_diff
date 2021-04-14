using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class BigMap<TKey, TValue>
	{
		public BigMap() : this(31, 71993)
		{
		}

		public BigMap(int internalStoreNumber, int initialCapacity = 71993)
		{
			this.concurrentDictionaries = new ConcurrentDictionary<TKey, TValue>[internalStoreNumber];
			for (int i = 0; i < internalStoreNumber; i++)
			{
				this.concurrentDictionaries[i] = new ConcurrentDictionary<TKey, TValue>(4 * Environment.ProcessorCount, initialCapacity);
			}
		}

		public DateTime TimeStamp { get; set; }

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		internal int NumberOfDictionary
		{
			get
			{
				return this.concurrentDictionaries.Length;
			}
		}

		public ConcurrentDictionary<TKey, TValue> GetDictionary(int i)
		{
			if (i >= this.NumberOfDictionary || i < 0)
			{
				throw new ArgumentOutOfRangeException("i");
			}
			return this.concurrentDictionaries[i];
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			ConcurrentDictionary<TKey, TValue> concurrentDictionary = this.concurrentDictionaries[(int)((UIntPtr)this.GetDictionaryIdx(key))];
			return concurrentDictionary.TryGetValue(key, out value);
		}

		public TValue GetOrAdd(TKey key, TValue value)
		{
			ConcurrentDictionary<TKey, TValue> concurrentDictionary = this.concurrentDictionaries[(int)((UIntPtr)this.GetDictionaryIdx(key))];
			TValue result;
			if (concurrentDictionary.TryGetValue(key, out result))
			{
				return result;
			}
			if (concurrentDictionary.TryAdd(key, value))
			{
				Interlocked.Increment(ref this.count);
				return value;
			}
			concurrentDictionary.TryGetValue(key, out result);
			return result;
		}

		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			ConcurrentDictionary<TKey, TValue> concurrentDictionary = this.concurrentDictionaries[(int)((UIntPtr)this.GetDictionaryIdx(key))];
			TValue result;
			if (concurrentDictionary.TryGetValue(key, out result))
			{
				return result;
			}
			TValue tvalue = valueFactory(key);
			if (concurrentDictionary.TryAdd(key, tvalue))
			{
				Interlocked.Increment(ref this.count);
				return tvalue;
			}
			concurrentDictionary.TryGetValue(key, out result);
			return result;
		}

		public TValue AddOrGet(TKey key, TValue value, Action postAdd)
		{
			ConcurrentDictionary<TKey, TValue> concurrentDictionary = this.concurrentDictionaries[(int)((UIntPtr)this.GetDictionaryIdx(key))];
			if (concurrentDictionary.TryAdd(key, value))
			{
				Interlocked.Increment(ref this.count);
				postAdd();
				return value;
			}
			TValue result;
			concurrentDictionary.TryGetValue(key, out result);
			return result;
		}

		public TValue AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> updateValueFactory)
		{
			ConcurrentDictionary<TKey, TValue> concurrentDictionary = this.concurrentDictionaries[(int)((UIntPtr)this.GetDictionaryIdx(key))];
			if (concurrentDictionary.TryAdd(key, value))
			{
				Interlocked.Increment(ref this.count);
				return value;
			}
			return concurrentDictionary.AddOrUpdate(key, value, updateValueFactory);
		}

		public void Clear()
		{
			for (int i = 0; i < this.NumberOfDictionary; i++)
			{
				this.concurrentDictionaries[i] = new ConcurrentDictionary<TKey, TValue>(4 * Environment.ProcessorCount, 71993);
			}
			this.count = 0;
		}

		private uint GetDictionaryIdx(TKey key)
		{
			if (this.NumberOfDictionary == 1)
			{
				return 0U;
			}
			return FnvHash.Fnv1A32(key.GetHashCode().ToString(CultureInfo.InvariantCulture)) % (uint)this.NumberOfDictionary;
		}

		public const int DefaultInternalStoreNumber = 31;

		public const int InitialCapacity = 71993;

		private const int DefaultConcurrencyMultiplier = 4;

		private readonly ConcurrentDictionary<TKey, TValue>[] concurrentDictionaries;

		private int count;
	}
}
