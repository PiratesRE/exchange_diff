using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal abstract class HierarchyBulkOperation : BulkOperation
	{
		public HierarchyBulkOperation(MapiFolder folder, bool processAssociatedDumpsterFolders, int chunkSize) : base(chunkSize)
		{
			this.principalFolder = folder;
			this.processAssociatedDumpsterFolders = processAssociatedDumpsterFolders;
		}

		protected MapiFolder PrincipalFolder
		{
			get
			{
				return this.principalFolder;
			}
		}

		public override bool DoChunk(MapiContext context, out bool progress, out bool incomplete, out ErrorCode error)
		{
			progress = false;
			incomplete = false;
			error = ErrorCode.NoError;
			if (this.stack == null)
			{
				this.stack = new Stack<HierarchyBulkOperation.FolderStackEntry>();
				if (!this.principalFolder.CheckAlive(context))
				{
					error = ErrorCode.CreateObjectDeleted((LID)33304U);
					return true;
				}
				this.stack.Push(new HierarchyBulkOperation.FolderStackEntry
				{
					FolderInfo = null,
					IsPrincipal = true,
					IsAssociatedDumpster = false,
					Expanded = false,
					ParentFolder = null,
					DestinationParentFolder = null
				});
				if (this.processAssociatedDumpsterFolders)
				{
					ExchangeId associatedFolderId = this.principalFolder.GetAssociatedFolderId(context);
					if (associatedFolderId != ExchangeId.Null)
					{
						this.stack.Push(new HierarchyBulkOperation.FolderStackEntry
						{
							FolderInfo = null,
							IsPrincipal = true,
							IsAssociatedDumpster = true,
							Expanded = false,
							ParentFolder = null,
							DestinationParentFolder = null
						});
					}
				}
			}
			this.chunkCount = 0;
			bool flag = false;
			while (this.chunkCount < base.ChunkSize && !flag)
			{
				if (this.stack.Count == 0)
				{
					flag = true;
					break;
				}
				HierarchyBulkOperation.FolderStackEntry folderStackEntry = this.stack.Pop();
				this.entryToDispose = folderStackEntry;
				if (!folderStackEntry.Expanded)
				{
					bool flag2;
					int num;
					if (!this.InitializeForFolder(context, folderStackEntry, out flag2, out num, ref incomplete, ref error))
					{
						flag = true;
						break;
					}
					if (num != 0)
					{
						this.chunkCount += num;
						progress = true;
					}
					if (!flag2)
					{
						folderStackEntry.Expanded = true;
						this.stack.Push(folderStackEntry);
						this.entryToDispose = null;
						if (folderStackEntry.ProcessSubfolders)
						{
							FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, this.PrincipalFolder.Logon.StoreMailbox, this.PrincipalFolder.Fid.ToExchangeShortId(), FolderInformationType.Basic);
							if (folderStackEntry.FolderInfo == null)
							{
								folderStackEntry.FolderInfo = folderHierarchy.Find(context, this.PrincipalFolder.Fid.ToExchangeShortId());
							}
							IList<IFolderInformation> children = folderHierarchy.GetChildren(context, folderStackEntry.FolderInfo);
							for (int i = children.Count - 1; i >= 0; i--)
							{
								this.stack.Push(new HierarchyBulkOperation.FolderStackEntry
								{
									FolderInfo = children[i],
									IsPrincipal = false,
									IsAssociatedDumpster = false,
									Expanded = false,
									ParentFolder = folderStackEntry.Folder,
									DestinationParentFolder = folderStackEntry.DestinationFolder
								});
								if (this.processAssociatedDumpsterFolders)
								{
									this.stack.Push(new HierarchyBulkOperation.FolderStackEntry
									{
										FolderInfo = null,
										IsPrincipal = false,
										IsAssociatedDumpster = true,
										Expanded = false,
										ParentFolder = null,
										DestinationParentFolder = null
									});
								}
							}
						}
					}
					else
					{
						folderStackEntry.Dispose();
						this.entryToDispose = null;
					}
				}
				else
				{
					if (!this.CheckDestinationFolder(context, folderStackEntry))
					{
						error = ErrorCode.CreateObjectDeleted((LID)49688U);
						flag = true;
						break;
					}
					if (this.CheckSourceFolder(context, folderStackEntry))
					{
						int num;
						if (!this.ContinueForFolder(context, folderStackEntry, out num, ref incomplete, ref error))
						{
							flag = true;
							break;
						}
						if (num != 0)
						{
							this.chunkCount += num;
							progress = true;
							this.stack.Push(folderStackEntry);
							this.entryToDispose = null;
							continue;
						}
						if (!this.FinishForFolder(context, folderStackEntry, out num, ref incomplete, ref error))
						{
							flag = true;
							break;
						}
						if (num != 0)
						{
							this.chunkCount += num;
							progress = true;
						}
					}
					else
					{
						incomplete = true;
					}
					folderStackEntry.Dispose();
					this.entryToDispose = null;
				}
			}
			return flag;
		}

		protected virtual bool CheckSourceFolder(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry)
		{
			return currentEntry.Folder.CheckAlive(context);
		}

		protected virtual bool CheckDestinationFolder(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry)
		{
			return currentEntry.DestinationFolder == null || currentEntry.DestinationFolder.CheckAlive(context);
		}

		private bool InitializeForFolder(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out bool skipFolder, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			IReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, context.LockedMailboxState);
			if (currentEntry.IsPrincipal)
			{
				if (currentEntry.IsAssociatedDumpster)
				{
					ExchangeId associatedFolderId = this.PrincipalFolder.GetAssociatedFolderId(context);
					currentEntry.FolderInfo = FolderHierarchy.FolderInformationFromFolderId(associatedFolderId.ToExchangeShortId());
					currentEntry.Folder = MapiFolder.OpenFolder(context, this.PrincipalFolder.Logon, associatedFolderId);
					currentEntry.DisposeFolder = true;
					if (currentEntry.Folder == null)
					{
						progressCount = 0;
						incomplete = true;
						skipFolder = true;
						error = ErrorCode.CreateNotFound((LID)51776U);
						return false;
					}
				}
				else
				{
					currentEntry.Folder = this.PrincipalFolder;
					currentEntry.DisposeFolder = false;
				}
			}
			else if (this.processAssociatedDumpsterFolders)
			{
				if (currentEntry.IsAssociatedDumpster)
				{
					HierarchyBulkOperation.FolderStackEntry folderStackEntry = this.stack.Peek();
					folderStackEntry.Folder = MapiFolder.OpenFolder(context, this.PrincipalFolder.Logon, ExchangeId.CreateFromInternalShortId(context, cacheForMailbox, folderStackEntry.FolderInfo.Fid));
					folderStackEntry.DisposeFolder = true;
					if (folderStackEntry.Folder != null)
					{
						ExchangeId associatedFolderId2 = folderStackEntry.Folder.GetAssociatedFolderId(context);
						if (associatedFolderId2 != ExchangeId.Null)
						{
							currentEntry.FolderInfo = FolderHierarchy.FolderInformationFromFolderId(associatedFolderId2.ToExchangeShortId());
							currentEntry.Folder = MapiFolder.OpenFolder(context, this.PrincipalFolder.Logon, associatedFolderId2);
							currentEntry.DisposeFolder = true;
							if (currentEntry.Folder == null)
							{
								progressCount = 0;
								incomplete = true;
								skipFolder = true;
								error = ErrorCode.CreateNotFound((LID)45632U);
								return false;
							}
						}
					}
					if (currentEntry.Folder == null)
					{
						progressCount = 0;
						incomplete = false;
						skipFolder = true;
						return true;
					}
				}
				else
				{
					if (currentEntry.Folder == null)
					{
						currentEntry.Folder = MapiFolder.OpenFolder(context, this.PrincipalFolder.Logon, ExchangeId.CreateFromInternalShortId(context, cacheForMailbox, currentEntry.FolderInfo.Fid));
						currentEntry.DisposeFolder = true;
					}
					if (currentEntry.Folder == null)
					{
						progressCount = 0;
						incomplete = true;
						skipFolder = true;
						return true;
					}
				}
			}
			else
			{
				currentEntry.Folder = MapiFolder.OpenFolder(context, this.PrincipalFolder.Logon, ExchangeId.CreateFromInternalShortId(context, cacheForMailbox, currentEntry.FolderInfo.Fid));
				currentEntry.DisposeFolder = true;
				if (currentEntry.Folder == null)
				{
					progressCount = 0;
					incomplete = true;
					skipFolder = true;
					return true;
				}
			}
			skipFolder = false;
			return this.ProcessFolderStart(context, currentEntry, out progressCount, ref incomplete, ref error);
		}

		private bool ContinueForFolder(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			progressCount = 0;
			int num = base.ChunkSize - this.chunkCount;
			while (currentEntry.ProcessAssociatedMessages || currentEntry.ProcessNormalMessages || currentEntry.ProcessingMessages)
			{
				if (progressCount >= num)
				{
					return true;
				}
				int numRows = Math.Min(num - progressCount, (base.ChunkSize + 1) / 2);
				if (currentEntry.MessageView == null)
				{
					ViewMessageConfigureFlags viewMessageConfigureFlags = ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.DoNotUseLazyIndex;
					if (currentEntry.ProcessAssociatedMessages && currentEntry.ProcessNormalMessages)
					{
						viewMessageConfigureFlags |= ViewMessageConfigureFlags.ViewAll;
					}
					else if (currentEntry.ProcessAssociatedMessages)
					{
						viewMessageConfigureFlags |= ViewMessageConfigureFlags.ViewFAI;
					}
					currentEntry.ProcessAssociatedMessages = false;
					currentEntry.ProcessNormalMessages = false;
					if (currentEntry.Folder.IsGhosted(context, (LID)46796U))
					{
						continue;
					}
					currentEntry.ProcessingMessages = true;
					currentEntry.MessageView = new MapiViewMessage();
					currentEntry.MessageView.Configure(context, this.PrincipalFolder.Logon, currentEntry.Folder, viewMessageConfigureFlags);
					currentEntry.MessageView.SetColumns(context, BulkOperation.ColumnsToFetchMid, MapiViewSetColumnsFlag.NoColumnValidation);
				}
				IList<Properties> list = currentEntry.MessageView.QueryRowsBatch(context, numRows, QueryRowsFlags.None);
				if (list == null || list.Count == 0)
				{
					currentEntry.ProcessingMessages = false;
					currentEntry.MessageView.Dispose();
					currentEntry.MessageView = null;
				}
				else
				{
					if (this.midsToProcess == null)
					{
						this.midsToProcess = new List<ExchangeId>(list.Count);
					}
					else
					{
						this.midsToProcess.Clear();
					}
					for (int i = 0; i < list.Count; i++)
					{
						this.midsToProcess.Add(ExchangeId.CreateFrom26ByteArray(context, this.PrincipalFolder.Logon.StoreMailbox.ReplidGuidMap, (byte[])list[i][0].Value));
					}
					int num2;
					if (!this.ProcessMessages(context, currentEntry, this.midsToProcess, out num2, ref incomplete, ref error))
					{
						return false;
					}
					progressCount += num2;
				}
			}
			return true;
		}

		private bool FinishForFolder(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			return this.ProcessFolderEnd(context, currentEntry, out progressCount, ref incomplete, ref error);
		}

		protected abstract bool ProcessFolderStart(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error);

		protected abstract bool ProcessMessages(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error);

		protected abstract bool ProcessFolderEnd(MapiContext context, HierarchyBulkOperation.FolderStackEntry currentEntry, out int progressCount, ref bool incomplete, ref ErrorCode error);

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				if (this.entryToDispose != null)
				{
					this.entryToDispose.Dispose();
					this.entryToDispose = null;
				}
				if (this.stack != null)
				{
					while (this.stack.Count != 0)
					{
						HierarchyBulkOperation.FolderStackEntry folderStackEntry = this.stack.Pop();
						folderStackEntry.Dispose();
					}
					this.stack = null;
				}
			}
		}

		private readonly MapiFolder principalFolder;

		private readonly bool processAssociatedDumpsterFolders;

		private Stack<HierarchyBulkOperation.FolderStackEntry> stack;

		private HierarchyBulkOperation.FolderStackEntry entryToDispose;

		private int chunkCount;

		private List<ExchangeId> midsToProcess;

		protected class FolderStackEntry : DisposableBase
		{
			protected override void InternalDispose(bool calledFromDispose)
			{
				if (this.DisposeFolder && this.Folder != null)
				{
					this.Folder.Dispose();
					this.Folder = null;
				}
				if (this.DisposeDestinationFolder && this.DestinationFolder != null)
				{
					this.DestinationFolder.Dispose();
					this.DestinationFolder = null;
				}
				if (this.MessageView != null)
				{
					this.MessageView.Dispose();
					this.MessageView = null;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<HierarchyBulkOperation.FolderStackEntry>(this);
			}

			public IFolderInformation FolderInfo;

			public bool IsPrincipal;

			public bool IsAssociatedDumpster;

			public bool Expanded;

			public MapiFolder ParentFolder;

			public MapiFolder Folder;

			public bool DisposeFolder;

			public bool ProcessSubfolders;

			public bool ProcessAssociatedMessages;

			public bool ProcessNormalMessages;

			public bool ProcessingMessages;

			public MapiFolder DestinationParentFolder;

			public MapiFolder DestinationFolder;

			public bool DisposeDestinationFolder;

			public MapiViewMessage MessageView;
		}
	}
}
