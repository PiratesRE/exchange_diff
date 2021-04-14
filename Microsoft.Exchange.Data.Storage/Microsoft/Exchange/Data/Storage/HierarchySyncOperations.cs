using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HierarchySyncOperations : IEnumerable<HierarchySyncOperation>, IEnumerable
	{
		internal HierarchySyncOperations(FolderHierarchySync folderHierarchySync, IDictionary changes, bool moreAvailable)
		{
			this.moreAvailable = moreAvailable;
			this.changes = new List<HierarchySyncOperation>(changes.Keys.Count);
			foreach (object obj in changes.Values)
			{
				FolderManifestEntry manifestEntry = (FolderManifestEntry)obj;
				HierarchySyncOperation hierarchySyncOperation = new HierarchySyncOperation();
				hierarchySyncOperation.Bind(folderHierarchySync, manifestEntry);
				this.changes.Add(hierarchySyncOperation);
			}
		}

		public bool MoreAvailable
		{
			get
			{
				return this.moreAvailable;
			}
		}

		public int Count
		{
			get
			{
				return this.changes.Count;
			}
		}

		public HierarchySyncOperation this[int idx]
		{
			get
			{
				return this.changes[idx];
			}
		}

		IEnumerator<HierarchySyncOperation> IEnumerable<HierarchySyncOperation>.GetEnumerator()
		{
			return this.changes.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.changes.GetEnumerator();
		}

		private List<HierarchySyncOperation> changes;

		private bool moreAvailable;
	}
}
