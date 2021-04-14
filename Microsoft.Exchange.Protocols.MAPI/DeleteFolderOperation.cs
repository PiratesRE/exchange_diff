using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class DeleteFolderOperation : HierarchyBulkOperation
	{
		public DeleteFolderOperation(MapiFolder folder, bool deleteSubfolders, bool deleteMessages, bool processAssociatedDumpsterFolders, bool force, int chunkSize) : base(folder, processAssociatedDumpsterFolders, chunkSize)
		{
			this.deleteSubfolders = deleteSubfolders;
			this.deleteMessages = deleteMessages;
			this.force = force;
		}

		public DeleteFolderOperation(MapiFolder folder, bool deleteSubfolders, bool deleteMessages, bool processAssociatedDumpsterFolders, bool force) : this(folder, deleteSubfolders, deleteMessages, processAssociatedDumpsterFolders, force, 100)
		{
		}

		protected override bool ProcessFolderStart(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			currentEntry.ProcessSubfolders = (this.deleteSubfolders && !currentEntry.IsAssociatedDumpster);
			if (currentEntry.Folder.IsSearchFolder())
			{
				currentEntry.ProcessAssociatedMessages = false;
				currentEntry.ProcessNormalMessages = false;
			}
			else
			{
				currentEntry.ProcessAssociatedMessages = (this.deleteMessages || !currentEntry.IsPrincipal);
				currentEntry.ProcessNormalMessages = (this.deleteMessages || !currentEntry.IsPrincipal);
			}
			if (currentEntry.ProcessNormalMessages || currentEntry.ProcessAssociatedMessages)
			{
				currentEntry.Folder.StoreFolder.InvalidateIndexes(context, currentEntry.ProcessNormalMessages, currentEntry.ProcessAssociatedMessages);
			}
			return true;
		}

		protected override bool ProcessMessages(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			return BulkOperation.DeleteMessages(context, currentEntry.Folder, true, this.force, midsToProcess, BulkErrorAction.Skip, BulkErrorAction.Incomplete, out progressCount, ref incomplete, ref error);
		}

		protected override bool ProcessFolderEnd(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			if (currentEntry.IsAssociatedDumpster)
			{
				progressCount = 0;
				return true;
			}
			error = currentEntry.Folder.Delete(context);
			if (error != ErrorCode.NoError)
			{
				progressCount = 0;
				return BulkOperation.ContinueOnError(ref error, ref incomplete, BulkErrorAction.Skip, BulkErrorAction.Incomplete);
			}
			progressCount = 1;
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DeleteFolderOperation>(this);
		}

		private bool deleteSubfolders;

		private bool deleteMessages;

		private bool force;
	}
}
