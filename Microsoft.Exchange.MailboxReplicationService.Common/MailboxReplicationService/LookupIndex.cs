using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class LookupIndex<TKey, TData> : LookupTable<TData>.IIndex where TData : class
	{
		public LookupIndex()
		{
			this.entries = new Dictionary<TKey, LookupIndexEntry<TData>>(this.GetEqualityComparer());
		}

		public LookupTable<TData> Owner
		{
			get
			{
				return this.owner;
			}
		}

		public List<TKey> ResolvedKeys
		{
			get
			{
				this.Owner.LookupUnresolvedKeys();
				List<TKey> list = new List<TKey>();
				foreach (KeyValuePair<TKey, LookupIndexEntry<TData>> keyValuePair in this.entries)
				{
					if (keyValuePair.Value.Data != null)
					{
						list.Add(keyValuePair.Key);
					}
				}
				return list;
			}
		}

		public TData this[TKey key]
		{
			get
			{
				LookupIndexEntry<TData> indexEntry = this.GetIndexEntry(key);
				this.Owner.LookupUnresolvedKeys();
				return indexEntry.Data;
			}
		}

		public void AddKey(TKey key)
		{
			this.GetIndexEntry(key);
		}

		public void AddKeys(ICollection<TKey> keys)
		{
			foreach (TKey key in keys)
			{
				this.AddKey(key);
			}
		}

		void LookupTable<!1>.IIndex.SetOwner(LookupTable<TData> owner)
		{
			this.owner = owner;
		}

		void LookupTable<!1>.IIndex.InsertObject(TData data)
		{
			ICollection<TKey> collection = this.RetrieveKeys(data);
			if (collection != null)
			{
				foreach (TKey key in collection)
				{
					LookupIndexEntry<TData> indexEntry = this.GetIndexEntry(key);
					indexEntry.Data = data;
					indexEntry.IsResolved = true;
				}
			}
		}

		void LookupTable<!1>.IIndex.LookupUnresolvedKeys()
		{
			List<TKey> list = null;
			foreach (KeyValuePair<TKey, LookupIndexEntry<TData>> keyValuePair in this.entries)
			{
				if (!keyValuePair.Value.IsResolved)
				{
					if (list == null)
					{
						list = new List<TKey>();
					}
					list.Add(keyValuePair.Key);
				}
			}
			if (list == null)
			{
				return;
			}
			TData[] array = this.LookupKeys(list.ToArray());
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == null)
				{
					LookupIndexEntry<TData> lookupIndexEntry = this.entries[list[i]];
					lookupIndexEntry.IsResolved = true;
					lookupIndexEntry.Data = default(TData);
				}
				else
				{
					this.Owner.InsertObject(array[i]);
				}
			}
		}

		void LookupTable<!1>.IIndex.Clear()
		{
			this.entries.Clear();
		}

		protected abstract ICollection<TKey> RetrieveKeys(TData data);

		protected abstract TData[] LookupKeys(TKey[] keys);

		protected virtual IEqualityComparer<TKey> GetEqualityComparer()
		{
			return null;
		}

		private LookupIndexEntry<TData> GetIndexEntry(TKey key)
		{
			LookupIndexEntry<TData> lookupIndexEntry;
			if (!this.entries.TryGetValue(key, out lookupIndexEntry))
			{
				lookupIndexEntry = new LookupIndexEntry<TData>();
				this.entries.Add(key, lookupIndexEntry);
			}
			return lookupIndexEntry;
		}

		private LookupTable<TData> owner;

		private Dictionary<TKey, LookupIndexEntry<TData>> entries;
	}
}
