using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Search
{
	internal class FolderNode
	{
		internal FolderNode(StoreId sourceFolderId, StoreId targetFolderId, string displayName, FolderNode parent) : this(sourceFolderId, targetFolderId, displayName, false, parent)
		{
		}

		internal FolderNode(StoreId sourceFolderId, StoreId targetFolderId, string displayName, bool isSoftDeleted, FolderNode parent)
		{
			this.SourceFolderId = sourceFolderId;
			this.TargetFolderId = targetFolderId;
			this.DisplayName = displayName;
			this.Parent = parent;
			this.IsSoftDeleted = isSoftDeleted;
		}

		internal FolderNode Parent
		{
			get
			{
				return this.parent;
			}
			set
			{
				this.parent = value;
			}
		}

		internal StoreId SourceFolderId
		{
			get
			{
				return this.sourceFolderId;
			}
			set
			{
				this.sourceFolderId = value;
			}
		}

		internal StoreId TargetFolderId
		{
			get
			{
				return this.targetFolderId;
			}
			set
			{
				this.targetFolderId = value;
			}
		}

		internal string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		internal bool IsSoftDeleted
		{
			get
			{
				return this.isSoftDeleted;
			}
			set
			{
				this.isSoftDeleted = value;
			}
		}

		private FolderNode parent;

		private StoreId sourceFolderId;

		private StoreId targetFolderId;

		private string displayName;

		private bool isSoftDeleted;
	}
}
