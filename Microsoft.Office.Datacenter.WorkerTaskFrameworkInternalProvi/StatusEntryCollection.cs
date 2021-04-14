using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public class StatusEntryCollection : IEnumerable<StatusEntry>, IEnumerable
	{
		public StatusEntryCollection(string collectionKey)
		{
			this.collectionKey = collectionKey;
		}

		public int Count
		{
			get
			{
				return this.items.Count;
			}
		}

		internal StatusEntry[] ItemsToRemove
		{
			get
			{
				return this.itemsToRemove.ToArray();
			}
		}

		public IEnumerator<StatusEntry> GetEnumerator()
		{
			foreach (StatusEntry item in (from i in this.items
			where !i.Remove
			select i).ToArray<StatusEntry>())
			{
				yield return item;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public StatusEntry CreateStatusEntry()
		{
			StatusEntry statusEntry = new StatusEntry(this.collectionKey);
			this.items.Add(statusEntry);
			return statusEntry;
		}

		public void Remove(StatusEntry entry)
		{
			lock (this.removalLock)
			{
				entry.Remove = true;
				this.itemsToRemove.Add(entry);
				this.items.Remove(entry);
			}
		}

		internal void Add(StatusEntry entry)
		{
			if (entry.Key != this.collectionKey)
			{
				throw new ArgumentException(string.Format("The entry's key '{0}' does not match the key assigned to this collection: '{1}'", entry.Key, this.collectionKey));
			}
			this.items.Add(entry);
		}

		internal void PurgeInvalidEntries()
		{
			lock (this.removalLock)
			{
				int i = 0;
				while (i < this.itemsToRemove.Count)
				{
					StatusEntry statusEntry = this.itemsToRemove[i];
					if (!statusEntry.EntryExistsInDatabase())
					{
						this.itemsToRemove.RemoveAt(i);
					}
					else
					{
						i++;
					}
				}
				int j = 0;
				while (j < this.items.Count)
				{
					StatusEntry statusEntry2 = this.items[j];
					if (DateTime.UtcNow.Subtract(statusEntry2.CreatedTime) > StatusEntryCollection.RetentionTime)
					{
						this.itemsToRemove.Add(this.items[j]);
						this.items.RemoveAt(j);
					}
					else
					{
						j++;
					}
				}
			}
		}

		private static readonly TimeSpan RetentionTime = TimeSpan.FromHours((double)(4 * Settings.ProbeResultHistoryWindowSize));

		private readonly string collectionKey;

		private List<StatusEntry> items = new List<StatusEntry>();

		private List<StatusEntry> itemsToRemove = new List<StatusEntry>();

		private object removalLock = new object();
	}
}
