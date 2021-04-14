using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal abstract class DefaultUMSearchFolder : UMSearchFolder
	{
		internal DefaultUMSearchFolder(MailboxSession itemStore) : base(itemStore)
		{
		}

		protected abstract DefaultFolderType DefaultFolderType { get; }

		protected override void InternalCreateSearchFolder()
		{
			StoreObjectId folderId = base.ItemStore.CreateDefaultFolder(this.DefaultFolderType);
			base.SearchFolder = SearchFolder.Bind(base.ItemStore, folderId);
		}

		protected override void InternalDeleteSearchFolder()
		{
			base.ItemStore.DeleteDefaultFolder(this.DefaultFolderType, DeleteItemFlags.SoftDelete);
		}

		protected override bool InternalTryRepairSearchFolder()
		{
			StoreObjectId folderId = null;
			if (base.ItemStore.TryFixDefaultFolderId(this.DefaultFolderType, out folderId))
			{
				base.SearchFolder = SearchFolder.Bind(base.ItemStore, folderId);
				ExTraceGlobals.UtilTracer.TraceDebug(0L, "Successfully repaired UM search folder.");
				return true;
			}
			ExTraceGlobals.UtilTracer.TraceError(0L, "Failed to repair UM search folder.");
			return false;
		}

		protected override StoreObjectId GetSearchFolderId()
		{
			return base.ItemStore.GetDefaultFolderId(this.DefaultFolderType);
		}
	}
}
