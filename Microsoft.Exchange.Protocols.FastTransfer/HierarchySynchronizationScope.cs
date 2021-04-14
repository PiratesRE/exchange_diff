using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.FastTransfer
{
	internal class HierarchySynchronizationScope : IHierarchySynchronizationScope
	{
		public HierarchySynchronizationScope(MapiFolder folder)
		{
			this.folder = folder;
		}

		public MapiLogon Logon
		{
			get
			{
				return this.folder.Logon;
			}
		}

		public MapiFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public ExchangeId GetExchangeId(long shortTermId)
		{
			return ExchangeId.CreateFromInt64(this.Logon.StoreMailbox.CurrentOperationContext, this.Logon.StoreMailbox.ReplidGuidMap, shortTermId);
		}

		public ReplId GuidToReplid(Guid guid)
		{
			return new ReplId(this.Logon.StoreMailbox.ReplidGuidMap.GetReplidFromGuid(this.Logon.StoreMailbox.CurrentOperationContext, guid));
		}

		public Guid ReplidToGuid(ReplId replid)
		{
			return this.Logon.StoreMailbox.ReplidGuidMap.GetGuidFromReplid(this.Logon.StoreMailbox.CurrentOperationContext, replid.Value);
		}

		public IdSet GetServerCnsetSeen(MapiContext operationContext)
		{
			return this.Logon.StoreMailbox.GetFolderCnsetIn(operationContext);
		}

		public void GetChangedAndDeletedFolders(MapiContext context, SyncFlag syncFlags, IdSet cnsetSeen, IdSet idsetGiven, out IList<FolderChangeEntry> changedFolders, out IdSet idsetNewDeletes)
		{
			ReplidGuidMap replidGuidMap = this.folder.Logon.StoreMailbox.ReplidGuidMap;
			bool flag;
			Restriction restriction = ContentSynchronizationScopeBase.CreateCnsetSeenRestriction(context, replidGuidMap, PropTag.Folder.ChangeNumberBin, cnsetSeen, false, out flag);
			FolderTable folderTable = Microsoft.Exchange.Server.Storage.LogicalDataModel.DatabaseSchema.FolderTable(this.folder.Logon.StoreMailbox.Database);
			int mailboxPartitionNumber = this.folder.Logon.StoreMailbox.MailboxPartitionNumber;
			IList<KeyRange> keyRanges;
			if (restriction != null)
			{
				SearchCriteria searchCriteria = restriction.ToSearchCriteria(this.folder.Logon.StoreMailbox.Database, Microsoft.Exchange.Server.Storage.PropTags.ObjectType.Folder);
				keyRanges = QueryPlanner.BuildKeyRangesFromOrCriteria(new object[]
				{
					mailboxPartitionNumber
				}, folderTable.FolderChangeNumberIndex, ref searchCriteria, false, context.Culture);
			}
			else
			{
				StartStopKey startStopKey = new StartStopKey(true, new object[]
				{
					mailboxPartitionNumber
				});
				keyRanges = new KeyRange[]
				{
					new KeyRange(startStopKey, startStopKey)
				};
			}
			List<FolderChangeEntry> list = new List<FolderChangeEntry>(100);
			List<int> list2 = new List<int>(100);
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, this.folder.Logon.StoreMailbox, this.folder.Fid.ToExchangeShortId(), FolderInformationType.Basic);
			using (TableOperator tableOperator = Factory.CreateTableOperator(context.Culture, context, folderTable.Table, folderTable.FolderChangeNumberIndex, new Column[]
			{
				folderTable.FolderId,
				folderTable.LcnCurrent
			}, null, null, 0, 0, keyRanges, false, true))
			{
				using (Reader reader = tableOperator.ExecuteReader(false))
				{
					while (reader.Read())
					{
						ExchangeId id = ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, reader.GetBinary(folderTable.LcnCurrent));
						if (!flag || !cnsetSeen.Contains(id))
						{
							int num;
							IFolderInformation folderInformation = folderHierarchy.Find(context, ExchangeId.CreateFrom26ByteArray(context, replidGuidMap, reader.GetBinary(folderTable.FolderId)).ToExchangeShortId(), out num);
							if (folderInformation != null && num != 0 && !folderInformation.IsSearchFolder && (!folderInformation.IsInternalAccess || context.HasInternalAccessRights))
							{
								list.Add(new FolderChangeEntry
								{
									FolderId = folderInformation.Fid,
									Cn = id.ToLong()
								});
								if ((ushort)(syncFlags & SyncFlag.CatchUp) == 0)
								{
									list2.Add(num);
								}
							}
						}
					}
				}
			}
			changedFolders = list;
			if ((ushort)(syncFlags & SyncFlag.CatchUp) == 0)
			{
				FolderChangeEntry[] array = list.ToArray();
				int[] keys = list2.ToArray();
				Array.Sort<int, FolderChangeEntry>(keys, array);
				changedFolders = array;
			}
			if ((ushort)(syncFlags & SyncFlag.NoDeletions) == 2 || (ushort)(syncFlags & SyncFlag.CatchUp) == 1024)
			{
				idsetNewDeletes = null;
				return;
			}
			if (idsetGiven.IsEmpty)
			{
				idsetNewDeletes = new IdSet();
				return;
			}
			idsetNewDeletes = IdSet.Subtract(idsetGiven, this.GetExistingFoldersSet(context));
		}

		private IdSet GetExistingFoldersSet(MapiContext context)
		{
			ExchangeId fid = this.folder.Fid;
			IdSet idSet = new IdSet();
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, this.folder.Logon.StoreMailbox, fid.ToExchangeShortId(), FolderInformationType.Basic);
			if (folderHierarchy != null && folderHierarchy.HierarchyRoots != null && folderHierarchy.HierarchyRoots.Count != 0)
			{
				IReplidGuidMap cacheForMailbox = ReplidGuidMap.GetCacheForMailbox(context, context.LockedMailboxState);
				foreach (ExchangeShortId exchangeShortId in folderHierarchy.HierarchyRoots[0].AllDescendantFolderIds())
				{
					Guid guid = cacheForMailbox.InternalGetGuidFromReplid(context, exchangeShortId.Replid);
					idSet.Insert(guid, exchangeShortId.Counter);
				}
			}
			return idSet;
		}

		public ExchangeId GetRootFid()
		{
			return this.folder.Fid;
		}

		public MapiFolder OpenFolder(ExchangeId fid)
		{
			return MapiFolder.OpenFolder(this.folder.CurrentOperationContext, this.Logon, fid);
		}

		private MapiFolder folder;

		private class SyncFolderInformation
		{
			internal ExchangeId FolderId
			{
				get
				{
					return this.folderId;
				}
				set
				{
					this.folderId = value;
				}
			}

			internal ExchangeId ParentFolderId
			{
				get
				{
					return this.parentFolderId;
				}
				set
				{
					this.parentFolderId = value;
				}
			}

			internal ExchangeId Cn
			{
				get
				{
					return this.cn;
				}
				set
				{
					this.cn = value;
				}
			}

			internal List<HierarchySynchronizationScope.SyncFolderInformation> Children
			{
				get
				{
					return this.children;
				}
			}

			public void LinkToParent(HierarchySynchronizationScope.SyncFolderInformation parent)
			{
				parent.Children.Add(this);
			}

			private readonly List<HierarchySynchronizationScope.SyncFolderInformation> children = new List<HierarchySynchronizationScope.SyncFolderInformation>();

			private ExchangeId folderId;

			private ExchangeId parentFolderId;

			private ExchangeId cn;
		}
	}
}
