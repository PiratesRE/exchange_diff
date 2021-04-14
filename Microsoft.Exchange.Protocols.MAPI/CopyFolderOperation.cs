using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class CopyFolderOperation : HierarchyBulkOperation
	{
		public CopyFolderOperation(MapiFolder sourceFolder, MapiFolder destinationParentFolder, string newFolderName, bool recurse, int chunkSize) : base(sourceFolder, false, chunkSize)
		{
			this.recurse = recurse;
			this.destinationPrincipalFolder = destinationParentFolder;
			this.newFolderName = newFolderName;
		}

		public CopyFolderOperation(MapiFolder sourceFolder, MapiFolder destinationParentFolder, string newFolderName, bool recurse) : this(sourceFolder, destinationParentFolder, newFolderName, recurse, 100)
		{
		}

		protected override bool ProcessFolderStart(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			if (currentEntry.IsPrincipal)
			{
				if (!this.destinationPrincipalFolder.CheckAlive(context))
				{
					progressCount = 0;
					error = ErrorCode.CreateObjectDeleted((LID)64920U);
					return false;
				}
				error = currentEntry.Folder.Copy(context, this.destinationPrincipalFolder, this.newFolderName, out currentEntry.DestinationFolder);
				currentEntry.DisposeDestinationFolder = true;
			}
			else
			{
				if (!currentEntry.DestinationParentFolder.CheckAlive(context))
				{
					progressCount = 0;
					error = ErrorCode.CreateObjectDeleted((LID)40344U);
					return false;
				}
				error = currentEntry.Folder.Copy(context, currentEntry.DestinationParentFolder, currentEntry.Folder.GetDisplayName(context), out currentEntry.DestinationFolder);
				currentEntry.DisposeDestinationFolder = true;
			}
			if (error != ErrorCode.NoError)
			{
				progressCount = 0;
				return false;
			}
			currentEntry.ProcessSubfolders = this.recurse;
			if (!currentEntry.Folder.IsSearchFolder())
			{
				currentEntry.ProcessAssociatedMessages = true;
				currentEntry.ProcessNormalMessages = true;
			}
			else
			{
				currentEntry.ProcessAssociatedMessages = false;
				currentEntry.ProcessNormalMessages = false;
			}
			progressCount = 1;
			return true;
		}

		protected override bool ProcessMessages(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			return BulkOperation.CopyMessages(context, currentEntry.Folder, currentEntry.DestinationFolder, midsToProcess, Properties.Empty, BulkErrorAction.Incomplete, BulkErrorAction.Error, null, null, out progressCount, ref incomplete, ref error);
		}

		protected override bool ProcessFolderEnd(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			return true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CopyFolderOperation>(this);
		}

		private MapiFolder destinationPrincipalFolder;

		private string newFolderName;

		private bool recurse;
	}
}
