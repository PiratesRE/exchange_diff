using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	public class CachedList<T, U>
	{
		public CachedList(CachedList<T, U>.IndividualUpdateMethod<T> updateMethod, TimeSpan expirationTime) : this(expirationTime)
		{
			this.isMassUpdate = false;
			this.individualUpdateMethod = updateMethod;
		}

		public CachedList(CachedList<T, U>.MassUpdateMethod<T> massUpdateMethod, TimeSpan expirationTime) : this(expirationTime)
		{
			this.isMassUpdate = true;
			this.massUpdateMethod = massUpdateMethod;
		}

		private CachedList(TimeSpan expirationTime)
		{
			this.expirationTime = expirationTime;
			this.cacheTable = new Dictionary<U, CachedList<T, U>.CachedValueStruct>();
			this.rwLock = new ReaderWriterLockSlim();
		}

		public T GetValue(U index)
		{
			this.rwLock.EnterUpgradeableReadLock();
			T value;
			try
			{
				this.UpdateCacheIfNecessary(new U[]
				{
					index
				});
				value = this.cacheTable[index].Value;
			}
			finally
			{
				this.rwLock.ExitUpgradeableReadLock();
			}
			return value;
		}

		public KeyValuePair<U, T>[] GetValues(params U[] indexes)
		{
			this.rwLock.EnterUpgradeableReadLock();
			KeyValuePair<U, T>[] result;
			try
			{
				this.UpdateCacheIfNecessary(indexes);
				List<KeyValuePair<U, T>> list = new List<KeyValuePair<U, T>>();
				foreach (U key in indexes)
				{
					list.Add(new KeyValuePair<U, T>(key, this.cacheTable[key].Value));
				}
				result = list.ToArray();
			}
			finally
			{
				this.rwLock.ExitUpgradeableReadLock();
			}
			return result;
		}

		private void UpdateCacheIfNecessary(params U[] indexes)
		{
			List<U> list = new List<U>();
			DateTime utcNow = DateTime.UtcNow;
			foreach (U u in indexes)
			{
				if (!this.cacheTable.ContainsKey(u) || utcNow - this.cacheTable[u].LastUpdate > this.expirationTime)
				{
					list.Add(u);
				}
			}
			if (list.Count > 0)
			{
				this.rwLock.EnterWriteLock();
				try
				{
					if (this.isMassUpdate)
					{
						KeyValuePair<U, T>[] array = this.massUpdateMethod(list.ToArray());
						foreach (KeyValuePair<U, T> keyValuePair in array)
						{
							if (this.cacheTable.ContainsKey(keyValuePair.Key))
							{
								this.cacheTable[keyValuePair.Key] = new CachedList<T, U>.CachedValueStruct
								{
									LastUpdate = utcNow,
									Value = keyValuePair.Value
								};
							}
							else
							{
								this.cacheTable.Add(keyValuePair.Key, new CachedList<T, U>.CachedValueStruct
								{
									LastUpdate = utcNow,
									Value = keyValuePair.Value
								});
							}
						}
					}
					else
					{
						foreach (U u2 in list)
						{
							if (this.cacheTable.ContainsKey(u2))
							{
								this.cacheTable[u2] = new CachedList<T, U>.CachedValueStruct
								{
									LastUpdate = utcNow,
									Value = this.individualUpdateMethod(u2)
								};
							}
							else
							{
								this.cacheTable.Add(u2, new CachedList<T, U>.CachedValueStruct
								{
									LastUpdate = utcNow,
									Value = this.individualUpdateMethod(u2)
								});
							}
						}
					}
				}
				finally
				{
					this.rwLock.ExitWriteLock();
				}
			}
		}

		private readonly TimeSpan expirationTime;

		private readonly bool isMassUpdate;

		private Dictionary<U, CachedList<T, U>.CachedValueStruct> cacheTable;

		private CachedList<T, U>.IndividualUpdateMethod<T> individualUpdateMethod;

		private CachedList<T, U>.MassUpdateMethod<T> massUpdateMethod;

		private ReaderWriterLockSlim rwLock;

		public delegate V IndividualUpdateMethod<V>(U index);

		public delegate KeyValuePair<U, W>[] MassUpdateMethod<W>(params U[] indexes);

		private struct CachedValueStruct
		{
			public T Value;

			public DateTime LastUpdate;
		}
	}
}
