using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class FolderCopyPropsOperation : HierarchyBulkOperation
	{
		public FolderCopyPropsOperation(MapiFolder sourceFolder, MapiFolder destinationFolder, StorePropTag[] propTags, bool replaceIfExists, int chunkSize) : base(sourceFolder, false, chunkSize)
		{
			this.destinationPrincipalFolder = destinationFolder;
			this.propTags = propTags;
			this.replaceIfExists = replaceIfExists;
		}

		public FolderCopyPropsOperation(MapiFolder sourceFolder, MapiFolder destinationFolder, StorePropTag[] propTags, bool replaceIfExists) : this(sourceFolder, destinationFolder, propTags, replaceIfExists, 100)
		{
		}

		public List<MapiPropertyProblem> PropertyProblems
		{
			get
			{
				return this.propertyProblems;
			}
		}

		protected override bool ProcessFolderStart(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			bool processSubfolders = true;
			bool processNormalMessages = true;
			bool processAssociatedMessages = true;
			if (currentEntry.IsPrincipal)
			{
				if (!this.destinationPrincipalFolder.CheckAlive(context))
				{
					progressCount = 0;
					error = ErrorCode.CreateObjectDeleted((LID)46488U);
					return false;
				}
				if (this.destinationPrincipalFolder.IsSearchFolder())
				{
					progressCount = 0;
					error = ErrorCode.CreateNotSupported((LID)62872U);
					return false;
				}
				if (currentEntry.Folder.Fid == this.destinationPrincipalFolder.Fid)
				{
					progressCount = 0;
					error = ErrorCode.CreateNoAccess((LID)38296U);
					return false;
				}
				List<StorePropTag> list = new List<StorePropTag>(this.propTags);
				processSubfolders = false;
				processNormalMessages = false;
				processAssociatedMessages = false;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].PropId == PropTag.Folder.ContainerContents.PropId)
					{
						processNormalMessages = true;
						list.RemoveAt(i);
						i--;
					}
					else if (list[i].PropId == PropTag.Folder.ContainerHierarchy.PropId)
					{
						processSubfolders = true;
						list.RemoveAt(i);
						i--;
					}
					else if (list[i].PropId == PropTag.Folder.FolderAssociatedContents.PropId)
					{
						processAssociatedMessages = true;
						list.RemoveAt(i);
						i--;
					}
				}
				currentEntry.Folder.CopyProps(context, this.destinationPrincipalFolder, list, this.replaceIfExists, ref this.propertyProblems);
				currentEntry.DestinationFolder = this.destinationPrincipalFolder;
				currentEntry.DisposeDestinationFolder = false;
			}
			else
			{
				if (!currentEntry.DestinationParentFolder.CheckAlive(context))
				{
					progressCount = 0;
					error = ErrorCode.CreateObjectDeleted((LID)54680U);
					return false;
				}
				error = currentEntry.Folder.Copy(context, currentEntry.DestinationParentFolder, currentEntry.Folder.GetDisplayName(context), out currentEntry.DestinationFolder);
				currentEntry.DisposeDestinationFolder = true;
				if (error != ErrorCode.NoError)
				{
					progressCount = 0;
					return false;
				}
			}
			currentEntry.ProcessSubfolders = processSubfolders;
			if (!currentEntry.Folder.IsSearchFolder())
			{
				currentEntry.ProcessAssociatedMessages = processAssociatedMessages;
				currentEntry.ProcessNormalMessages = processNormalMessages;
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
			return DisposeTracker.Get<FolderCopyPropsOperation>(this);
		}

		private MapiFolder destinationPrincipalFolder;

		private StorePropTag[] propTags;

		private bool replaceIfExists;

		private List<MapiPropertyProblem> propertyProblems;
	}
}
