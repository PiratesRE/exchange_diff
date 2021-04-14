using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class FolderChange : ChangedItem
	{
		public Dictionary<Guid, FileChange> FileChanges
		{
			get
			{
				return this.fileChanges;
			}
		}

		public bool HasFileChangesOnly
		{
			get
			{
				return base.Id == Guid.Empty;
			}
		}

		public IEnumerator<FileChange> FileChangesEnumerator { get; private set; }

		public StoreObjectId FolderId { get; set; }

		public FolderChange(Uri authority, Guid id, string version, string relativePath, string leafNode, ExDateTime whenCreated, ExDateTime lastModified) : base(authority, id, version, relativePath, leafNode, whenCreated, lastModified)
		{
			if (!this.HasFileChangesOnly && string.IsNullOrEmpty(leafNode))
			{
				throw new ArgumentException("LeafNode can't be null when there is a new folder change");
			}
		}

		public void Reset()
		{
			this.FolderId = null;
			this.FileChangesEnumerator = this.fileChanges.Values.GetEnumerator();
		}

		public FolderChange Clone()
		{
			FolderChange folderChange = new FolderChange(base.Authority, base.Id, base.Version, base.RelativePath, base.LeafNode, base.WhenCreated, base.LastModified);
			foreach (KeyValuePair<Guid, FileChange> keyValuePair in this.FileChanges)
			{
				folderChange.FileChanges.Add(keyValuePair.Key, keyValuePair.Value);
			}
			folderChange.Reset();
			return folderChange;
		}

		private readonly Dictionary<Guid, FileChange> fileChanges = new Dictionary<Guid, FileChange>();
	}
}
