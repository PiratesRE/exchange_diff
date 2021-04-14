using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class HierarchySyncOperation
	{
		internal HierarchySyncOperation()
		{
		}

		public ChangeType ChangeType
		{
			get
			{
				return this.manifestEntry.ChangeType;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ChangeType>(value, "value");
				this.manifestEntry.ChangeType = value;
			}
		}

		public StoreObjectId ItemId
		{
			get
			{
				return this.manifestEntry.ItemId;
			}
		}

		public StoreObjectId ParentId
		{
			get
			{
				return this.manifestEntry.ParentId;
			}
		}

		public bool IsSharedFolder
		{
			get
			{
				return !string.IsNullOrEmpty(this.manifestEntry.Owner);
			}
		}

		public SyncPermissions Permissions
		{
			get
			{
				return this.manifestEntry.Permissions;
			}
		}

		public string Owner
		{
			get
			{
				return this.manifestEntry.Owner;
			}
		}

		public bool Hidden
		{
			get
			{
				return this.manifestEntry.Hidden;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.manifestEntry.DisplayName;
			}
		}

		public string ClassName
		{
			get
			{
				return this.manifestEntry.ClassName;
			}
		}

		public Folder GetFolder(params PropertyDefinition[] prefetchProperties)
		{
			return this.folderHierarchySync.GetFolder(this.manifestEntry, prefetchProperties);
		}

		public Folder GetFolder()
		{
			return this.folderHierarchySync.GetFolder(this.manifestEntry, null);
		}

		internal void Bind(FolderHierarchySync folderHierarchySync, FolderManifestEntry manifestEntry)
		{
			this.manifestEntry = manifestEntry;
			this.folderHierarchySync = folderHierarchySync;
		}

		private FolderHierarchySync folderHierarchySync;

		private FolderManifestEntry manifestEntry;
	}
}
