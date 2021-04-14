using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class EmptyFolderOperation : HierarchyBulkOperation
	{
		public EmptyFolderOperation(MapiFolder folder, bool deleteAssociatedMessages, bool processAssociatedDumpsterFolders, bool force, int chunkSize) : base(folder, processAssociatedDumpsterFolders, chunkSize)
		{
			this.deleteAssociatedMessages = deleteAssociatedMessages;
			this.force = force;
		}

		public EmptyFolderOperation(MapiFolder folder, bool deleteAssociatedMessages, bool processAssociatedDumpsterFolders, bool force) : this(folder, deleteAssociatedMessages, processAssociatedDumpsterFolders, force, 100)
		{
		}

		protected override bool ProcessFolderStart(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			currentEntry.ProcessSubfolders = !currentEntry.IsAssociatedDumpster;
			if (currentEntry.IsPrincipal && base.PrincipalFolder.IsSearchFolder())
			{
				progressCount = 0;
				error = ErrorCode.CreateNotSupported((LID)48536U);
				return false;
			}
			if (currentEntry.Folder.IsSearchFolder())
			{
				currentEntry.ProcessAssociatedMessages = false;
				currentEntry.ProcessNormalMessages = false;
			}
			else
			{
				currentEntry.ProcessAssociatedMessages = (this.deleteAssociatedMessages || !currentEntry.IsPrincipal);
				currentEntry.ProcessNormalMessages = true;
			}
			if (currentEntry.ProcessNormalMessages || currentEntry.ProcessAssociatedMessages)
			{
				currentEntry.Folder.StoreFolder.InvalidateIndexes(context, currentEntry.ProcessNormalMessages, currentEntry.ProcessAssociatedMessages);
			}
			progressCount = 0;
			return true;
		}

		protected override bool ProcessMessages(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			return BulkOperation.DeleteMessages(context, currentEntry.Folder, true, this.force, midsToProcess, BulkErrorAction.Skip, BulkErrorAction.Incomplete, out progressCount, ref incomplete, ref error);
		}

		protected override bool ProcessFolderEnd(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			if (currentEntry.IsAssociatedDumpster)
			{
				return true;
			}
			if (!currentEntry.IsPrincipal)
			{
				error = currentEntry.Folder.Delete(context);
				if (error != ErrorCode.NoError)
				{
					return BulkOperation.ContinueOnError(ref error, ref incomplete, BulkErrorAction.Skip, BulkErrorAction.Incomplete);
				}
				progressCount = 1;
			}
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EmptyFolderOperation>(this);
		}

		private bool deleteAssociatedMessages;

		private bool force;
	}
}
