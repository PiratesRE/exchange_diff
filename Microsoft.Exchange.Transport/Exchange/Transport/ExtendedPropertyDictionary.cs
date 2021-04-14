using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport
{
	internal sealed class ExtendedPropertyDictionary : DataInternalComponent, IExtendedPropertyCollection, IReadOnlyExtendedPropertyCollection, IDictionary<string, object>, ICollection<KeyValuePair<string, object>>, IEnumerable<KeyValuePair<string, object>>, IDictionary, ICollection, IEnumerable
	{
		public ExtendedPropertyDictionary(DataRow dataRow, DataColumn column) : base(dataRow)
		{
			if (column == null)
			{
				throw new ArgumentNullException("column");
			}
			this.column = column;
			ExtendedPropertyDictionary.InitValidTypes();
		}

		public ExtendedPropertyDictionary(DataRow dataRow, BlobCollection blobCollection, byte blobCollectionKey) : base(dataRow)
		{
			if (blobCollection == null)
			{
				throw new ArgumentNullException("blobCollection");
			}
			this.blobCollection = blobCollection;
			this.blobCollectionKey = blobCollectionKey;
			ExtendedPropertyDictionary.InitValidTypes();
		}

		public ExtendedPropertyDictionary() : base(null)
		{
			ExtendedPropertyDictionary.InitValidTypes();
		}

		public int Count
		{
			get
			{
				if (this.data != null || this.pendingDeferredLoad)
				{
					return this.Data.Count;
				}
				return 0;
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return ((IDictionary<string, object>)this.Data).Keys;
			}
		}

		public ICollection<object> Values
		{
			get
			{
				return ((IDictionary<string, object>)this.Data).Values;
			}
		}

		public override bool PendingDatabaseUpdates
		{
			get
			{
				return this.Dirty;
			}
		}

		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
			set
			{
				this.dirty = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		ICollection IDictionary.Keys
		{
			get
			{
				return ((IDictionary)this.Data).Keys;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				return ((IDictionary)this.Data).Values;
			}
		}

		bool IDictionary.IsFixedSize
		{
			get
			{
				return false;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this;
			}
		}

		object IDictionary.this[object key]
		{
			get
			{
				object result;
				this.Data.TryGetValue((string)key, out result);
				return result;
			}
			set
			{
				this[(string)key] = value;
			}
		}

		private Dictionary<string, object> Data
		{
			get
			{
				Dictionary<string, object> dictionary = this.data;
				if (this.pendingDeferredLoad || dictionary == null)
				{
					lock (this)
					{
						if (this.pendingDeferredLoad)
						{
							this.DeferredLoad();
							this.pendingDeferredLoad = false;
						}
						else if (this.data == null)
						{
							this.data = this.CreateDictionary(ExtendedPropertyDictionary.initialDictionaryCapacity);
						}
						dictionary = this.data;
					}
				}
				return dictionary;
			}
		}

		public object this[string key]
		{
			get
			{
				return this.Data[key];
			}
			set
			{
				this.ThrowIfReadOnlyOrDeleted();
				ExtendedPropertyDictionary.ValidateValue(value);
				ExtendedPropertyDictionary.ValidateKey(key);
				if (this.HasChanged(key, value))
				{
					bool shouldRelease = false;
					try
					{
						shouldRelease = this.AssertOwnerAndTakeOwnership();
						this.Data[key] = value;
						this.dirty = true;
					}
					finally
					{
						this.AssertOwnerAndReleaseOwnership(shouldRelease);
					}
				}
			}
		}

		public void Serialize(Stream stream)
		{
			byte[] array = ExtendedPropertyDictionary.bufferPool.Acquire();
			try
			{
				int num = 0;
				ExtendedPropertyDictionary.initialDictionaryCapacity = this.Count;
				foreach (KeyValuePair<string, object> keyValuePair in this.Data)
				{
					TypedValue typedValue = KeySerializer.Serialize(keyValuePair.Key);
					int num2 = TransportPropertyStreamWriter.SizeOf(typedValue.Type, typedValue.Value);
					StreamPropertyType streamPropertyType = ExtendedPropertyDictionary.SerializeType(keyValuePair.Value);
					num2 += TransportPropertyStreamWriter.SizeOf(streamPropertyType, keyValuePair.Value);
					if (num2 > array.Length - num)
					{
						stream.Write(array, 0, num);
						num = 0;
						if (num2 > array.Length)
						{
							if (array.Length == 1024)
							{
								ExtendedPropertyDictionary.bufferPool.Release(array);
							}
							array = new byte[num2 * 2];
						}
					}
					TransportPropertyStreamWriter.Serialize(typedValue.Type, typedValue.Value, array, ref num);
					TransportPropertyStreamWriter.Serialize(streamPropertyType, keyValuePair.Value, array, ref num);
				}
				if (num > 0)
				{
					stream.Write(array, 0, num);
				}
			}
			finally
			{
				if (array.Length == 1024)
				{
					ExtendedPropertyDictionary.bufferPool.Release(array);
				}
			}
		}

		public void Deserialize(Stream stream, int numberOfPropertiesToFetch, bool doNotAddPropertyIfPresent)
		{
			this.ThrowIfReadOnlyOrDeleted();
			ExtendedPropertyDictionary.Deserialize(stream, this, numberOfPropertiesToFetch, doNotAddPropertyIfPresent);
		}

		public void Add(string key, object value)
		{
			this.ThrowIfReadOnlyOrDeleted();
			ExtendedPropertyDictionary.ValidateValue(value);
			ExtendedPropertyDictionary.ValidateKey(key);
			bool shouldRelease = false;
			try
			{
				shouldRelease = this.AssertOwnerAndTakeOwnership();
				this.Data.Add(key, value);
				this.dirty = true;
			}
			finally
			{
				this.AssertOwnerAndReleaseOwnership(shouldRelease);
			}
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			this.ThrowIfReadOnlyOrDeleted();
			this.Data.Add(item.Key, item.Value);
		}

		public void Clear()
		{
			this.ThrowIfReadOnlyOrDeleted();
			bool shouldRelease = false;
			try
			{
				shouldRelease = this.AssertOwnerAndTakeOwnership();
				if (this.pendingDeferredLoad)
				{
					this.pendingDeferredLoad = false;
					this.ClearData();
					this.dirty = true;
				}
				else if (this.data != null)
				{
					this.ClearData();
					this.dirty = true;
				}
			}
			finally
			{
				this.AssertOwnerAndReleaseOwnership(shouldRelease);
			}
		}

		public bool ContainsKey(string key)
		{
			return this.Data.ContainsKey(key);
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			return ((ICollection<KeyValuePair<string, object>>)this.Data).Contains(item);
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<string, object>>)this.Data).CopyTo(array, arrayIndex);
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<string, object>>)this.Data).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.Data).GetEnumerator();
		}

		public bool Remove(string key)
		{
			this.ThrowIfReadOnlyOrDeleted();
			bool shouldRelease = false;
			bool flag;
			try
			{
				shouldRelease = this.AssertOwnerAndTakeOwnership();
				flag = this.Data.Remove(key);
				if (flag)
				{
					this.dirty = true;
				}
			}
			finally
			{
				this.AssertOwnerAndReleaseOwnership(shouldRelease);
			}
			return flag;
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			return this.Remove(item.Key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return this.Data.TryGetValue(key, out value);
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			Type typeFromHandle = typeof(T);
			StreamPropertyType streamPropertyType;
			if (ExtendedPropertyDictionary.validTypes.TryGetValue(typeFromHandle, out streamPropertyType))
			{
				if ((short)(streamPropertyType & StreamPropertyType.Array) != 0 || (short)(streamPropertyType & StreamPropertyType.List) != 0)
				{
					throw new InvalidOperationException("Array or list property requested using TryGetValue<T>(); should use TryGetListValue<ItemT>() instead");
				}
			}
			else
			{
				if (typeFromHandle.Equals(typeof(object)))
				{
					throw new InvalidOperationException("Using SystemObject as type parameter is not allowed; should use strong types instead");
				}
				streamPropertyType = StreamPropertyType.Null;
			}
			object obj;
			if (this.TryGetValue(key, out obj) && (obj is T || (default(T) == null && obj == null)))
			{
				if (obj != null)
				{
					if (streamPropertyType == StreamPropertyType.Null)
					{
						StreamPropertyType streamPropertyType2 = ExtendedPropertyDictionary.validTypes[obj.GetType()];
						if ((short)(streamPropertyType2 & StreamPropertyType.Array) != 0 || (short)(streamPropertyType2 & StreamPropertyType.List) != 0)
						{
							throw new InvalidOperationException("Array or list property requested using TryGetValue<interface<T>>(); should use TryGetListValue<ItemT>() instead");
						}
						if (streamPropertyType2 == StreamPropertyType.IPEndPoint)
						{
							streamPropertyType = StreamPropertyType.IPEndPoint;
						}
					}
					if (streamPropertyType == StreamPropertyType.IPEndPoint)
					{
						IPEndPoint ipendPoint = (IPEndPoint)obj;
						obj = new IPEndPoint(ipendPoint.Address, ipendPoint.Port);
					}
				}
				value = (T)((object)obj);
				return true;
			}
			value = default(T);
			return false;
		}

		public bool TryGetListValue<ItemT>(string key, out ReadOnlyCollection<ItemT> value)
		{
			value = null;
			object obj;
			if (this.TryGetValue(key, out obj) && (obj == null || obj is IList<ItemT>))
			{
				if (obj != null)
				{
					value = new ReadOnlyCollection<ItemT>((IList<ItemT>)obj);
				}
				return true;
			}
			return false;
		}

		public T GetValue<T>(string name, T defaultValue)
		{
			T result;
			if (!this.TryGetValue<T>(name, out result))
			{
				return defaultValue;
			}
			return result;
		}

		public void SetValue<T>(string name, T value)
		{
			this[name] = value;
		}

		public bool Contains(string name)
		{
			return this.ContainsKey(name);
		}

		public override void LoadFromParentRow(DataTableCursor cursor)
		{
			this.ThrowIfReadOnlyOrDeleted();
			this.pendingDeferredLoad = true;
			this.ClearData();
			this.dirty = false;
		}

		public override void SaveToParentRow(DataTableCursor cursor, Func<bool> checkpointCallback)
		{
			if (this.PendingDatabaseUpdates)
			{
				using (Stream stream = (this.blobCollection == null) ? this.column.OpenImmediateWriter(cursor, base.DataRow, false, 1) : this.blobCollection.OpenWriter(this.blobCollectionKey, cursor, false, false, null))
				{
					stream.SetLength(0L);
					if (this.data != null && this.data.Count > 0)
					{
						bool shouldRelease = false;
						try
						{
							shouldRelease = this.AssertOwnerAndTakeOwnership();
							this.Serialize(stream);
						}
						finally
						{
							this.AssertOwnerAndReleaseOwnership(shouldRelease);
						}
						base.DataRow.PerfCounters.ExtendedPropertyBytesWritten.IncrementBy(stream.Length);
					}
					base.DataRow.PerfCounters.ExtendedPropertyWrites.Increment();
				}
				this.dirty = false;
			}
		}

		public override void CloneFrom(IDataObjectComponent other)
		{
			this.ThrowIfReadOnlyOrDeleted();
			if (other == null)
			{
				throw new ArgumentNullException("other");
			}
			ExtendedPropertyDictionary extendedPropertyDictionary = (ExtendedPropertyDictionary)other;
			this.PrepareEmptyData(extendedPropertyDictionary.Count);
			foreach (KeyValuePair<string, object> keyValuePair in extendedPropertyDictionary.Data)
			{
				object obj = keyValuePair.Value;
				StreamPropertyType streamPropertyType = (obj != null) ? ExtendedPropertyDictionary.validTypes[obj.GetType()] : StreamPropertyType.Null;
				StreamPropertyType streamPropertyType2 = streamPropertyType;
				if (streamPropertyType2 != StreamPropertyType.IPEndPoint)
				{
					switch (streamPropertyType2)
					{
					case StreamPropertyType.Bool | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array:
					case StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						obj = ((Array)obj).Clone();
						break;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						IPEndPoint[] array = (IPEndPoint[])obj;
						IPEndPoint[] array2 = new IPEndPoint[array.Length];
						for (int i = 0; i < array.Length; i++)
						{
							array2[i] = new IPEndPoint(array[i].Address, array[i].Port);
						}
						obj = array2;
						break;
					}
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						break;
					default:
						switch (streamPropertyType2)
						{
						case StreamPropertyType.Bool | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<bool>(obj as IEnumerable<bool>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<byte>(obj as IEnumerable<byte>);
							break;
						case StreamPropertyType.SByte | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<sbyte>(obj as IEnumerable<sbyte>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<short>(obj as IEnumerable<short>);
							break;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<ushort>(obj as IEnumerable<ushort>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<int>(obj as IEnumerable<int>);
							break;
						case StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<uint>(obj as IEnumerable<uint>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<long>(obj as IEnumerable<long>);
							break;
						case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<ulong>(obj as IEnumerable<ulong>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<float>(obj as IEnumerable<float>);
							break;
						case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<double>(obj as IEnumerable<double>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<decimal>(obj as IEnumerable<decimal>);
							break;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<char>(obj as IEnumerable<char>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<string>(obj as IEnumerable<string>);
							break;
						case StreamPropertyType.DateTime | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<DateTime>(obj as IEnumerable<DateTime>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<Guid>(obj as IEnumerable<Guid>);
							break;
						case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<IPAddress>(obj as IEnumerable<IPAddress>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
						{
							List<IPEndPoint> list = (List<IPEndPoint>)obj;
							List<IPEndPoint> list2 = new List<IPEndPoint>(list.Count);
							for (int j = 0; j < list.Count; j++)
							{
								list2[j] = new IPEndPoint(list[j].Address, list[j].Port);
							}
							obj = list2;
							break;
						}
						case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<RoutingAddress>(obj as IEnumerable<RoutingAddress>);
							break;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
							obj = ExtendedPropertyDictionary.CloneList<ADObjectId>(obj as IEnumerable<ADObjectId>);
							break;
						}
						break;
					}
				}
				else
				{
					IPEndPoint ipendPoint = (IPEndPoint)obj;
					obj = new IPEndPoint(ipendPoint.Address, ipendPoint.Port);
				}
				this.data.Add(keyValuePair.Key, obj);
			}
			this.dirty = extendedPropertyDictionary.dirty;
			this.pendingDeferredLoad = false;
		}

		public override string ToString()
		{
			int count = this.Count;
			if (count == 0)
			{
				return "{}";
			}
			StringBuilder stringBuilder = new StringBuilder(250 * count);
			int num = 0;
			stringBuilder.Append('{');
			foreach (KeyValuePair<string, object> keyValuePair in this.data)
			{
				if (num++ > 0)
				{
					stringBuilder.Append(',');
				}
				stringBuilder.Append(keyValuePair.Key);
				stringBuilder.Append('=');
				stringBuilder.Append(keyValuePair.Value.ToString());
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		bool IDictionary.Contains(object key)
		{
			return this.ContainsKey((string)key);
		}

		void IDictionary.Add(object key, object value)
		{
			string text = key as string;
			if (text == null)
			{
				throw new ArgumentException("The key must be of type string.", "key");
			}
			this.Add(text, value);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)this.Data).GetEnumerator();
		}

		void IDictionary.Remove(object key)
		{
			this.Remove((string)key);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)this.Data).CopyTo(array, index);
		}

		public override void MinimizeMemory()
		{
			if (!Monitor.TryEnter(this))
			{
				return;
			}
			try
			{
				if (this.data != null && !this.dirty)
				{
					if (this.data.Count == 0)
					{
						this.data = null;
					}
					else
					{
						this.pendingDeferredLoad = true;
						this.ClearData();
					}
				}
			}
			finally
			{
				Monitor.Exit(this);
			}
		}

		private static void InitValidTypes()
		{
			if (ExtendedPropertyDictionary.validTypes == null)
			{
				Dictionary<Type, StreamPropertyType> dictionary = new Dictionary<Type, StreamPropertyType>(ExtendedPropertyDictionary.SupportedTypes.Length);
				foreach (TypeEntry typeEntry in ExtendedPropertyDictionary.SupportedTypes)
				{
					dictionary.Add(typeEntry.Type, typeEntry.Identifier);
				}
				Interlocked.CompareExchange<Dictionary<Type, StreamPropertyType>>(ref ExtendedPropertyDictionary.validTypes, dictionary, null);
			}
		}

		private static void Deserialize(Stream stream, IDictionary<string, object> dst, int numberOfPropertiesToFetch, bool doNotAddPropertyIfPresent)
		{
			TransportPropertyStreamReader transportPropertyStreamReader = new TransportPropertyStreamReader(stream);
			int num = 0;
			KeyValuePair<string, object> keyValuePair;
			while (num != numberOfPropertiesToFetch && transportPropertyStreamReader.Read(out keyValuePair))
			{
				num++;
				if ((!doNotAddPropertyIfPresent || !dst.ContainsKey(keyValuePair.Key)) && !string.IsNullOrEmpty(keyValuePair.Key))
				{
					if (dst.ContainsKey(keyValuePair.Key))
					{
						dst[keyValuePair.Key] = keyValuePair.Value;
					}
					else
					{
						dst.Add(keyValuePair.Key, keyValuePair.Value);
					}
				}
			}
		}

		private static List<T> CloneList<T>(IEnumerable<T> source)
		{
			return new List<T>(source);
		}

		private static void ValidateValue(object value)
		{
			if (value == null)
			{
				return;
			}
			Type type = value.GetType();
			if (ExtendedPropertyDictionary.validTypes.ContainsKey(type))
			{
				return;
			}
			throw new ArgumentOutOfRangeException("value", value, "value is of a type that is not assignable to the dictionary.");
		}

		private static void ValidateKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (key.Length > 255)
			{
				throw new ArgumentException(Strings.KeyLength, "key");
			}
		}

		private static StreamPropertyType SerializeType(object value)
		{
			if (value == null)
			{
				return StreamPropertyType.Null;
			}
			StreamPropertyType result;
			if (ExtendedPropertyDictionary.validTypes.TryGetValue(value.GetType(), out result))
			{
				return result;
			}
			throw new InvalidOperationException("Unexpected type");
		}

		private bool HasChanged(string key, object value)
		{
			object obj;
			return !this.TryGetValue(key, out obj) || ((obj != null || value != null) && (obj == null || !obj.Equals(value)));
		}

		private void DeferredLoad()
		{
			bool shouldRelease = false;
			try
			{
				shouldRelease = this.AssertOwnerAndTakeOwnership();
				using (DataTableCursor cursor = base.DataRow.Table.GetCursor())
				{
					using (cursor.BeginTransaction())
					{
						base.DataRow.SeekCurrent(cursor);
						using (Stream stream = (this.blobCollection == null) ? this.column.OpenImmediateReader(cursor, base.DataRow, 1) : this.blobCollection.OpenReader(this.blobCollectionKey, cursor, false))
						{
							if (stream.Length > 0L)
							{
								byte[] array = new byte[stream.Length];
								stream.Read(array, 0, (int)stream.Length);
								using (MemoryStream memoryStream = new MemoryStream(array, false))
								{
									this.PrepareEmptyData(ExtendedPropertyDictionary.initialDictionaryCapacity);
									ExtendedPropertyDictionary.Deserialize(memoryStream, this.data, int.MaxValue, false);
								}
								base.DataRow.PerfCounters.ExtendedPropertyReads.Increment();
								base.DataRow.PerfCounters.ExtendedPropertyBytesRead.IncrementBy((long)array.Length);
							}
							else
							{
								this.PrepareEmptyData(ExtendedPropertyDictionary.initialDictionaryCapacity);
							}
							this.dirty = false;
						}
					}
				}
			}
			finally
			{
				this.AssertOwnerAndReleaseOwnership(shouldRelease);
			}
		}

		private bool AssertOwnerAndTakeOwnership()
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			int num = Interlocked.Exchange(ref this.ownerThread, managedThreadId);
			if (num == 0)
			{
				return true;
			}
			if (num != managedThreadId)
			{
				throw new ExtendedPropertyDictionary.InstrumentationException("Concurrent access detected", this, num);
			}
			return false;
		}

		private void AssertOwnerAndReleaseOwnership(bool shouldRelease)
		{
			int managedThreadId = Thread.CurrentThread.ManagedThreadId;
			int num;
			if (shouldRelease)
			{
				num = Interlocked.Exchange(ref this.ownerThread, 0);
			}
			else
			{
				num = this.ownerThread;
			}
			if (num != managedThreadId)
			{
				throw new ExtendedPropertyDictionary.InstrumentationException("Concurrent access detected", this, num);
			}
		}

		private void ClearData()
		{
			if (this.data != null)
			{
				this.data.Clear();
				this.data = null;
			}
		}

		private void PrepareEmptyData(int recommendedInitialCapacity)
		{
			if (this.data == null)
			{
				this.data = this.CreateDictionary(recommendedInitialCapacity);
				return;
			}
			this.data.Clear();
		}

		private Dictionary<string, object> CreateDictionary(int initialCapacity)
		{
			return new Dictionary<string, object>(initialCapacity, StringComparer.OrdinalIgnoreCase);
		}

		private void ThrowIfReadOnlyOrDeleted()
		{
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("This extended property operation cannot be performed in read-only mode.");
			}
			if (base.DataRow != null && base.DataRow.IsDeleted)
			{
				throw new InvalidOperationException("This extended property operation cannot be performed in a deleted item.");
			}
		}

		private const int MaxPropNameLength = 255;

		private const int PooledBufferSize = 1024;

		private static readonly TypeEntry[] SupportedTypes = new TypeEntry[]
		{
			new TypeEntry(typeof(DBNull), StreamPropertyType.Null),
			new TypeEntry(typeof(bool), StreamPropertyType.Bool),
			new TypeEntry(typeof(byte), StreamPropertyType.Byte),
			new TypeEntry(typeof(sbyte), StreamPropertyType.SByte),
			new TypeEntry(typeof(short), StreamPropertyType.Int16),
			new TypeEntry(typeof(ushort), StreamPropertyType.UInt16),
			new TypeEntry(typeof(int), StreamPropertyType.Int32),
			new TypeEntry(typeof(uint), StreamPropertyType.UInt32),
			new TypeEntry(typeof(long), StreamPropertyType.Int64),
			new TypeEntry(typeof(ulong), StreamPropertyType.UInt64),
			new TypeEntry(typeof(float), StreamPropertyType.Single),
			new TypeEntry(typeof(double), StreamPropertyType.Double),
			new TypeEntry(typeof(decimal), StreamPropertyType.Decimal),
			new TypeEntry(typeof(char), StreamPropertyType.Char),
			new TypeEntry(typeof(string), StreamPropertyType.String),
			new TypeEntry(typeof(DateTime), StreamPropertyType.DateTime),
			new TypeEntry(typeof(Guid), StreamPropertyType.Guid),
			new TypeEntry(typeof(IPAddress), StreamPropertyType.IPAddress),
			new TypeEntry(typeof(IPEndPoint), StreamPropertyType.IPEndPoint),
			new TypeEntry(typeof(RoutingAddress), StreamPropertyType.RoutingAddress),
			new TypeEntry(typeof(ADObjectId), StreamPropertyType.ADObjectIdUTF8),
			new TypeEntry(typeof(Microsoft.Exchange.Data.Directory.Recipient.RecipientType), StreamPropertyType.RecipientType),
			new TypeEntry(typeof(bool[]), StreamPropertyType.Bool | StreamPropertyType.Array),
			new TypeEntry(typeof(byte[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array),
			new TypeEntry(typeof(sbyte[]), StreamPropertyType.SByte | StreamPropertyType.Array),
			new TypeEntry(typeof(short[]), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array),
			new TypeEntry(typeof(ushort[]), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array),
			new TypeEntry(typeof(int[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array),
			new TypeEntry(typeof(uint[]), StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(long[]), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(ulong[]), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(float[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(double[]), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(decimal[]), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(char[]), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(string[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array),
			new TypeEntry(typeof(DateTime[]), StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(Guid[]), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(IPAddress[]), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(IPEndPoint[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(RoutingAddress[]), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(ADObjectId[]), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array),
			new TypeEntry(typeof(List<bool>), StreamPropertyType.Bool | StreamPropertyType.List),
			new TypeEntry(typeof(List<byte>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List),
			new TypeEntry(typeof(List<sbyte>), StreamPropertyType.SByte | StreamPropertyType.List),
			new TypeEntry(typeof(List<short>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List),
			new TypeEntry(typeof(List<ushort>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List),
			new TypeEntry(typeof(List<int>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List),
			new TypeEntry(typeof(List<uint>), StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<long>), StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<ulong>), StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<float>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<double>), StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<decimal>), StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<char>), StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<string>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List),
			new TypeEntry(typeof(List<DateTime>), StreamPropertyType.DateTime | StreamPropertyType.List),
			new TypeEntry(typeof(List<Guid>), StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List),
			new TypeEntry(typeof(List<IPAddress>), StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List),
			new TypeEntry(typeof(List<IPEndPoint>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List),
			new TypeEntry(typeof(List<RoutingAddress>), StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List),
			new TypeEntry(typeof(List<ADObjectId>), StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List)
		};

		private static readonly BufferPool bufferPool = new BufferPool(1024, true);

		private static Dictionary<Type, StreamPropertyType> validTypes;

		private static int initialDictionaryCapacity = 3;

		private readonly DataColumn column;

		private readonly byte blobCollectionKey;

		private readonly BlobCollection blobCollection;

		private volatile Dictionary<string, object> data;

		private int ownerThread;

		private volatile bool pendingDeferredLoad;

		private bool dirty;

		private bool isReadOnly;

		public class InstrumentationException : Exception
		{
			public InstrumentationException(string msg, object victim, int otherThreadId) : base(msg)
			{
				this.Victim = victim;
				this.ThisThreadId = Thread.CurrentThread.ManagedThreadId;
				this.OtherThreadId = otherThreadId;
			}

			public object Victim;

			public int ThisThreadId;

			public int OtherThreadId;
		}
	}
}
