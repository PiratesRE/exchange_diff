using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SyncOperations : IEnumerable<SyncOperation>, IEnumerable
	{
		public SyncOperations(FolderSync folderSync, IDictionary changes, bool moreAvailable)
		{
			this.moreAvailable = moreAvailable;
			this.changes = new List<SyncOperation>(changes.Keys.Count);
			foreach (object obj in changes.Values)
			{
				ServerManifestEntry serverManifestEntry = (ServerManifestEntry)obj;
				if (serverManifestEntry.ChangeType != ChangeType.OutOfFilter)
				{
					SyncOperation syncOperation = new SyncOperation();
					syncOperation.Bind(folderSync, serverManifestEntry, false);
					this.changes.Add(syncOperation);
				}
			}
		}

		public int Count
		{
			get
			{
				return this.changes.Count;
			}
		}

		public bool MoreAvailable
		{
			get
			{
				return this.moreAvailable;
			}
		}

		public SyncOperation this[int idx]
		{
			get
			{
				return this.changes[idx];
			}
		}

		public void RemoveAt(int index)
		{
			this.changes.RemoveAt(index);
		}

		IEnumerator<SyncOperation> IEnumerable<SyncOperation>.GetEnumerator()
		{
			return this.changes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.changes.GetEnumerator();
		}

		private List<SyncOperation> changes;

		private bool moreAvailable;
	}
}
