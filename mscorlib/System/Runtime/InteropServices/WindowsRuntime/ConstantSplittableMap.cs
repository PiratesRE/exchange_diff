using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	internal sealed class ConstantSplittableMap<TKey, TValue> : IMapView<TKey, TValue>, IIterable<IKeyValuePair<TKey, TValue>>, IEnumerable<IKeyValuePair<TKey, TValue>>, IEnumerable
	{
		internal ConstantSplittableMap(IReadOnlyDictionary<TKey, TValue> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.firstItemIndex = 0;
			this.lastItemIndex = data.Count - 1;
			this.items = this.CreateKeyValueArray(data.Count, data.GetEnumerator());
		}

		internal ConstantSplittableMap(IMapView<TKey, TValue> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (2147483647U < data.Size)
			{
				Exception ex = new InvalidOperationException(Environment.GetResourceString("InvalidOperation_CollectionBackingDictionaryTooLarge"));
				ex.SetErrorCode(-2147483637);
				throw ex;
			}
			int size = (int)data.Size;
			this.firstItemIndex = 0;
			this.lastItemIndex = size - 1;
			this.items = this.CreateKeyValueArray(size, data.GetEnumerator());
		}

		private ConstantSplittableMap(KeyValuePair<TKey, TValue>[] items, int firstItemIndex, int lastItemIndex)
		{
			this.items = items;
			this.firstItemIndex = firstItemIndex;
			this.lastItemIndex = lastItemIndex;
		}

		private KeyValuePair<TKey, TValue>[] CreateKeyValueArray(int count, IEnumerator<KeyValuePair<TKey, TValue>> data)
		{
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[count];
			int num = 0;
			while (data.MoveNext())
			{
				KeyValuePair<!0, !1> keyValuePair = data.Current;
				array[num++] = keyValuePair;
			}
			Array.Sort<KeyValuePair<TKey, TValue>>(array, ConstantSplittableMap<TKey, TValue>.keyValuePairComparator);
			return array;
		}

		private KeyValuePair<TKey, TValue>[] CreateKeyValueArray(int count, IEnumerator<IKeyValuePair<TKey, TValue>> data)
		{
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[count];
			int num = 0;
			while (data.MoveNext())
			{
				IKeyValuePair<TKey, TValue> keyValuePair = data.Current;
				array[num++] = new KeyValuePair<TKey, TValue>(keyValuePair.Key, keyValuePair.Value);
			}
			Array.Sort<KeyValuePair<TKey, TValue>>(array, ConstantSplittableMap<TKey, TValue>.keyValuePairComparator);
			return array;
		}

		public int Count
		{
			get
			{
				return this.lastItemIndex - this.firstItemIndex + 1;
			}
		}

		public uint Size
		{
			get
			{
				return (uint)(this.lastItemIndex - this.firstItemIndex + 1);
			}
		}

		public TValue Lookup(TKey key)
		{
			TValue result;
			if (!this.TryGetValue(key, out result))
			{
				Exception ex = new KeyNotFoundException(Environment.GetResourceString("Arg_KeyNotFound"));
				ex.SetErrorCode(-2147483637);
				throw ex;
			}
			return result;
		}

		public bool HasKey(TKey key)
		{
			TValue tvalue;
			return this.TryGetValue(key, out tvalue);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IKeyValuePair<TKey, TValue>>)this).GetEnumerator();
		}

		public IIterator<IKeyValuePair<TKey, TValue>> First()
		{
			return new EnumeratorToIteratorAdapter<IKeyValuePair<TKey, TValue>>(this.GetEnumerator());
		}

		public IEnumerator<IKeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return new ConstantSplittableMap<TKey, TValue>.IKeyValuePairEnumerator(this.items, this.firstItemIndex, this.lastItemIndex);
		}

		public void Split(out IMapView<TKey, TValue> firstPartition, out IMapView<TKey, TValue> secondPartition)
		{
			if (this.Count < 2)
			{
				firstPartition = null;
				secondPartition = null;
				return;
			}
			int num = (int)(((long)this.firstItemIndex + (long)this.lastItemIndex) / 2L);
			firstPartition = new ConstantSplittableMap<TKey, TValue>(this.items, this.firstItemIndex, num);
			secondPartition = new ConstantSplittableMap<TKey, TValue>(this.items, num + 1, this.lastItemIndex);
		}

		public bool ContainsKey(TKey key)
		{
			KeyValuePair<TKey, TValue> value = new KeyValuePair<TKey, TValue>(key, default(TValue));
			int num = Array.BinarySearch<KeyValuePair<TKey, TValue>>(this.items, this.firstItemIndex, this.Count, value, ConstantSplittableMap<TKey, TValue>.keyValuePairComparator);
			return num >= 0;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			KeyValuePair<TKey, TValue> value2 = new KeyValuePair<TKey, TValue>(key, default(TValue));
			int num = Array.BinarySearch<KeyValuePair<TKey, TValue>>(this.items, this.firstItemIndex, this.Count, value2, ConstantSplittableMap<TKey, TValue>.keyValuePairComparator);
			if (num < 0)
			{
				value = default(TValue);
				return false;
			}
			value = this.items[num].Value;
			return true;
		}

		public TValue this[TKey key]
		{
			get
			{
				return this.Lookup(key);
			}
		}

		public IEnumerable<TKey> Keys
		{
			get
			{
				throw new NotImplementedException("NYI");
			}
		}

		public IEnumerable<TValue> Values
		{
			get
			{
				throw new NotImplementedException("NYI");
			}
		}

		private static readonly ConstantSplittableMap<TKey, TValue>.KeyValuePairComparator keyValuePairComparator = new ConstantSplittableMap<TKey, TValue>.KeyValuePairComparator();

		private readonly KeyValuePair<TKey, TValue>[] items;

		private readonly int firstItemIndex;

		private readonly int lastItemIndex;

		private class KeyValuePairComparator : IComparer<KeyValuePair<TKey, TValue>>
		{
			public int Compare(KeyValuePair<TKey, TValue> x, KeyValuePair<TKey, TValue> y)
			{
				return ConstantSplittableMap<TKey, TValue>.KeyValuePairComparator.keyComparator.Compare(x.Key, y.Key);
			}

			private static readonly IComparer<TKey> keyComparator = Comparer<TKey>.Default;
		}

		[Serializable]
		internal struct IKeyValuePairEnumerator : IEnumerator<IKeyValuePair<TKey, TValue>>, IDisposable, IEnumerator
		{
			internal IKeyValuePairEnumerator(KeyValuePair<TKey, TValue>[] items, int first, int end)
			{
				this._array = items;
				this._start = first;
				this._end = end;
				this._current = this._start - 1;
			}

			public bool MoveNext()
			{
				if (this._current < this._end)
				{
					this._current++;
					return true;
				}
				return false;
			}

			public IKeyValuePair<TKey, TValue> Current
			{
				get
				{
					if (this._current < this._start)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
					}
					if (this._current > this._end)
					{
						throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
					}
					return new CLRIKeyValuePairImpl<TKey, TValue>(ref this._array[this._current]);
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}

			void IEnumerator.Reset()
			{
				this._current = this._start - 1;
			}

			public void Dispose()
			{
			}

			private KeyValuePair<TKey, TValue>[] _array;

			private int _start;

			private int _end;

			private int _current;
		}
	}
}
