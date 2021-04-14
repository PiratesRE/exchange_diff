using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Collections
{
	internal sealed class MruDictionaryCache<TToken, TData> : IDisposable
	{
		public MruDictionaryCache(int capacity, int expireTimeInMinutes) : this(capacity, capacity, expireTimeInMinutes)
		{
		}

		public MruDictionaryCache(int initialCapacity, int capacity, int expireTimeInMinutes) : this(initialCapacity, capacity, expireTimeInMinutes, null)
		{
		}

		public MruDictionaryCache(int initialCapacity, int capacity, int expireTimeInMinutes, Action<TToken, TData> onEntryExpired)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", initialCapacity, "InitialCapacity must be greater or equal 0");
			}
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException("capacity", capacity, "Capacity must be larger than 0");
			}
			if (expireTimeInMinutes < 1)
			{
				throw new ArgumentOutOfRangeException("expireTimeInMinutes", expireTimeInMinutes, "Expire times must be larger than 0");
			}
			this.dataFromToken = new Dictionary<TToken, LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo>>(initialCapacity);
			this.mruEntries = new LinkedList<MruDictionaryCache<TToken, TData>.EntryInfo>();
			this.expireTime = TimeSpan.FromMinutes((double)expireTimeInMinutes);
			this.onEntryExpired = onEntryExpired;
			this.expiryTimer = new Timer(new TimerCallback(this.ExpireEntries), null, (int)this.expireTime.TotalMilliseconds, (int)this.expireTime.TotalMilliseconds);
			this.capacity = capacity;
		}

		public int Count
		{
			get
			{
				int count;
				lock (this.dictionarySynchronizationObject)
				{
					count = this.dataFromToken.Count;
				}
				return count;
			}
		}

		public List<MruCacheDiagnosticEntryInfo> GetDiagnosticsInfo(Func<TData, string> dataToStringDelegate)
		{
			List<MruCacheDiagnosticEntryInfo> list;
			lock (this.dictionarySynchronizationObject)
			{
				list = new List<MruCacheDiagnosticEntryInfo>(this.dataFromToken.Count);
				DateTime d = DateTime.UtcNow.Subtract(this.expireTime);
				foreach (KeyValuePair<TToken, LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo>> keyValuePair in this.dataFromToken)
				{
					list.Add(new MruCacheDiagnosticEntryInfo(dataToStringDelegate(keyValuePair.Value.Value.NonExtendingData), keyValuePair.Value.Value.LastAccessed - d));
				}
			}
			return list;
		}

		public Action OnExpirationStart
		{
			get
			{
				return this.onExpirationStart;
			}
			set
			{
				this.onExpirationStart = value;
			}
		}

		public Action OnExpirationComplete
		{
			get
			{
				return this.onExpirationComplete;
			}
			set
			{
				this.onExpirationComplete = value;
			}
		}

		public bool ContainsKey(TToken token)
		{
			bool result;
			lock (this.dictionarySynchronizationObject)
			{
				result = this.dataFromToken.ContainsKey(token);
			}
			return result;
		}

		public int AddAndCount(TToken token, TData data)
		{
			int count;
			lock (this.dictionarySynchronizationObject)
			{
				this[token] = data;
				count = this.dataFromToken.Count;
			}
			return count;
		}

		public TData this[TToken key]
		{
			get
			{
				TData result;
				if (this.TryGetValue(key, out result))
				{
					return result;
				}
				throw new KeyNotFoundException();
			}
			set
			{
				TData data;
				lock (this.dictionarySynchronizationObject)
				{
					LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> linkedListNode;
					if (this.dataFromToken.TryGetValue(key, out linkedListNode))
					{
						this.UpdateRecentlyUsedNode(linkedListNode);
						data = linkedListNode.Value.SetEntryInformation(key, value);
					}
					else
					{
						data = this.InternalAdd(key, value);
					}
				}
				MruDictionaryCache<TToken, TData>.DisposeData(data);
			}
		}

		public bool TryGetValue(TToken token, out TData data)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> linkedListNode = null;
			lock (this.dictionarySynchronizationObject)
			{
				if (this.dataFromToken.TryGetValue(token, out linkedListNode))
				{
					data = linkedListNode.Value.Data;
					this.UpdateRecentlyUsedNode(linkedListNode);
					return true;
				}
			}
			data = default(TData);
			return false;
		}

		public bool TryGetAndRemoveValue(TToken token, out TData data)
		{
			lock (this.dictionarySynchronizationObject)
			{
				if (this.TryGetValue(token, out data))
				{
					this.Remove(token);
					return true;
				}
			}
			data = default(TData);
			return false;
		}

		public void Add(TToken token, TData data)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			TData data2 = default(TData);
			try
			{
				lock (this.dictionarySynchronizationObject)
				{
					data2 = this.InternalAdd(token, data);
				}
			}
			finally
			{
				MruDictionaryCache<TToken, TData>.DisposeData(data2);
			}
		}

		public TData AddAndReturnOriginalData(TToken token, TData data)
		{
			if (token == null)
			{
				throw new ArgumentNullException("token");
			}
			TData result;
			lock (this.dictionarySynchronizationObject)
			{
				result = this.InternalAdd(token, data);
			}
			return result;
		}

		public bool Remove(TToken key)
		{
			bool result;
			lock (this.dictionarySynchronizationObject)
			{
				LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> node = null;
				if (this.dataFromToken.TryGetValue(key, out node))
				{
					this.mruEntries.Remove(node);
				}
				result = this.dataFromToken.Remove(key);
			}
			return result;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		internal static void DisposeData(TData data)
		{
			IDisposable disposable = data as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.expiryTimer.Dispose();
					this.expiryTimer = null;
					lock (this.dictionarySynchronizationObject)
					{
						this.mruEntries.Clear();
						this.mruEntries = null;
						foreach (LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> linkedListNode in this.dataFromToken.Values)
						{
							linkedListNode.Value.DisposeData();
						}
						this.dataFromToken.Clear();
						this.dataFromToken = null;
					}
				}
				this.disposed = true;
			}
		}

		private void UpdateRecentlyUsedNode(LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> entryInfoNode)
		{
			this.mruEntries.Remove(entryInfoNode);
			this.mruEntries.AddFirst(entryInfoNode);
		}

		private void ExpireEntries(object state)
		{
			if (Interlocked.Exchange(ref this.expiringEntries, 1) != 1)
			{
				DateTime expirationTime = DateTime.UtcNow.Subtract(this.expireTime);
				try
				{
					bool flag = true;
					List<MruDictionaryCache<TToken, TData>.EntryInfo> list = new List<MruDictionaryCache<TToken, TData>.EntryInfo>(MruDictionaryCache<TToken, TData>.ExpireChunkSize);
					do
					{
						try
						{
							if (this.OnExpirationStart != null)
							{
								this.OnExpirationStart();
							}
							list.Clear();
							lock (this.dictionarySynchronizationObject)
							{
								for (int i = 0; i < MruDictionaryCache<TToken, TData>.ExpireChunkSize; i++)
								{
									LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> last = this.mruEntries.Last;
									if (last == null || !last.Value.IsExpired(expirationTime))
									{
										flag = false;
										break;
									}
									if (this.onEntryExpired != null)
									{
										this.onEntryExpired(last.Value.Token, last.Value.Data);
									}
									list.Add(last.Value);
									this.dataFromToken.Remove(last.Value.Token);
									this.mruEntries.Remove(last);
								}
							}
						}
						finally
						{
							try
							{
								if (this.OnExpirationComplete != null)
								{
									this.OnExpirationComplete();
								}
							}
							finally
							{
								foreach (MruDictionaryCache<TToken, TData>.EntryInfo entryInfo in list)
								{
									entryInfo.DisposeData();
								}
								list.Clear();
							}
						}
					}
					while (flag);
				}
				finally
				{
					Interlocked.Exchange(ref this.expiringEntries, 0);
				}
			}
		}

		private TData InternalAdd(TToken token, TData data)
		{
			LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo> linkedListNode;
			if (this.mruEntries.Count < this.capacity)
			{
				linkedListNode = this.mruEntries.AddFirst(new MruDictionaryCache<TToken, TData>.EntryInfo());
			}
			else
			{
				linkedListNode = this.mruEntries.Last;
				this.UpdateRecentlyUsedNode(linkedListNode);
				this.dataFromToken.Remove(linkedListNode.Value.Token);
			}
			this.dataFromToken[token] = linkedListNode;
			return linkedListNode.Value.SetEntryInformation(token, data);
		}

		private static readonly int ExpireChunkSize = 100;

		private LinkedList<MruDictionaryCache<TToken, TData>.EntryInfo> mruEntries;

		private Dictionary<TToken, LinkedListNode<MruDictionaryCache<TToken, TData>.EntryInfo>> dataFromToken;

		private object dictionarySynchronizationObject = new object();

		private int expiringEntries;

		private Timer expiryTimer;

		private Action<TToken, TData> onEntryExpired;

		private Action onExpirationStart;

		private Action onExpirationComplete;

		private int capacity;

		private bool disposed;

		private TimeSpan expireTime;

		private class EntryInfo
		{
			public TData Data
			{
				get
				{
					this.lastAccessed = DateTime.UtcNow;
					return this.data;
				}
			}

			public TToken Token
			{
				get
				{
					return this.token;
				}
			}

			public bool IsExpired(DateTime expirationTime)
			{
				return this.lastAccessed < expirationTime;
			}

			public TData SetEntryInformation(TToken token, TData data)
			{
				TData result = default(TData);
				this.token = token;
				if (!object.ReferenceEquals(this.data, data))
				{
					result = this.data;
					this.data = data;
				}
				this.lastAccessed = DateTime.UtcNow;
				return result;
			}

			public void DisposeData()
			{
				MruDictionaryCache<TToken, TData>.DisposeData(this.data);
			}

			internal TData NonExtendingData
			{
				get
				{
					return this.data;
				}
			}

			internal DateTime LastAccessed
			{
				get
				{
					return this.lastAccessed;
				}
			}

			private TData data;

			private TToken token;

			private DateTime lastAccessed;
		}
	}
}
