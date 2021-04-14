using System;
using System.Collections.Generic;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class LogicalMailbox : Mailbox
	{
		private LogicalMailbox(StoreDatabase database, MailboxState mailboxState, Context context) : base(database, mailboxState, context)
		{
		}

		private LogicalMailbox(StoreDatabase database, Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove) : base(database, context, mailboxState, mailboxDirectoryInfo, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, createdByMove)
		{
		}

		internal new static void Initialize()
		{
			Mailbox.SetMailboxInitializationDelegates((StoreDatabase database, MailboxState mailboxState, Context context) => new LogicalMailbox(database, mailboxState, context), (StoreDatabase database, Context context, MailboxState mailboxState, MailboxInfo mailboxDirectoryInfo, Guid mailboxInstanceGuid, Guid localIdGuid, Guid mappingSignatureGuid, ulong nextIdCounter, uint? reservedIdCounterRange, ulong nextCnCounter, uint? reservedCnCounterRange, Dictionary<ushort, StoreNamedPropInfo> numberToNameMap, Dictionary<ushort, Guid> replidGuidMap, Guid defaultFoldersReplGuid, bool createdByMove) => new LogicalMailbox(database, context, mailboxState, mailboxDirectoryInfo, mailboxInstanceGuid, localIdGuid, mappingSignatureGuid, nextIdCounter, reservedIdCounterRange, nextCnCounter, reservedCnCounterRange, numberToNameMap, replidGuidMap, defaultFoldersReplGuid, createdByMove));
		}

		public ExchangeId GetMaterializedRestrictionRootForFolder(Context context, ExchangeId searchedFolderId)
		{
			ExchangeId[] specialFolders = SpecialFoldersCache.GetSpecialFolders(context, this);
			bool flag = false;
			switch (ConfigurationSchema.MaterializedRestrictionSearchFolderConfigStage.Value)
			{
			case 0:
				break;
			case 1:
			case 2:
				if (EnableAddingSpecialFolders.IsReady(context, context.Database))
				{
					try
					{
						ExchangeId exchangeId = specialFolders[21];
						Folder folder;
						if (exchangeId.IsNullOrZero)
						{
							folder = Folder.CreateFolder(context, this);
							folder.SetName(context, "MaterializedRestrictions");
							folder.SetSpecialFolderNumber(context, SpecialFolders.MaterializedRestrictionRoot);
							folder.Save(context);
							flag = true;
							SpecialFoldersCache.Reset(context, this);
							exchangeId = folder.GetId(context);
						}
						if (ConfigurationSchema.MaterializedRestrictionSearchFolderConfigStage.Value == 1)
						{
							return exchangeId;
						}
						Folder folder2 = Folder.OpenFolder(context, this, searchedFolderId);
						if (folder2 == null)
						{
							return exchangeId;
						}
						byte[] array = folder2.GetPropertyValue(context, PropTag.Folder.MaterializedRestrictionSearchRoot) as byte[];
						ExchangeId exchangeId2;
						Folder folder3;
						if (array != null)
						{
							exchangeId2 = ExchangeId.CreateFrom26ByteArray(context, this.ReplidGuidMap, array);
							folder3 = Folder.OpenFolder(context, this, exchangeId2);
							if (folder3 != null)
							{
								return exchangeId2;
							}
						}
						folder = Folder.OpenFolder(context, this, exchangeId);
						folder3 = Folder.CreateFolder(context, folder);
						folder3.SetName(context, string.Format("{0:X16}", searchedFolderId.ToLong()));
						folder3.Save(context);
						flag = true;
						exchangeId2 = folder3.GetId(context);
						folder2.SetProperty(context, PropTag.Folder.MaterializedRestrictionSearchRoot, exchangeId2.To26ByteArray());
						folder2.Save(context);
						return exchangeId2;
					}
					finally
					{
						if (flag)
						{
							base.Save(context);
						}
					}
					goto IL_172;
				}
				break;
			default:
				goto IL_172;
			}
			return specialFolders[2];
			IL_172:
			Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, "unknown configuration stage for materialized restrictions");
			return ExchangeId.Zero;
		}

		public static void CreateOrDeleteIMAPIndices(Context context, MailboxState mailboxState, bool wantIndex)
		{
			if (mailboxState.Database.PhysicalDatabase.DatabaseType == DatabaseType.Jet && mailboxState.Database.IsOnlineActive)
			{
				MessageTable messageTable = DatabaseSchema.MessageTable(mailboxState.Database);
				Index[] array = new Index[]
				{
					messageTable.IMAPIDUnique,
					messageTable.ArticleNumberUnique
				};
				foreach (Index index in array)
				{
					bool flag = messageTable.Table.IsIndexCreated(context, index, new List<object>
					{
						mailboxState.MailboxPartitionNumber
					});
					if (wantIndex != flag)
					{
						if (wantIndex)
						{
							messageTable.Table.CreateIndex(context, index, new List<object>
							{
								mailboxState.MailboxPartitionNumber
							});
						}
						else
						{
							messageTable.Table.DeleteIndex(context, index, new List<object>
							{
								mailboxState.MailboxPartitionNumber
							});
						}
					}
				}
			}
		}

		protected override void RemoveCrossMailboxReferences(Context context)
		{
			if (base.SharedState.UnifiedState == null)
			{
				return;
			}
			IReplidGuidMap replidGuidMap = ReplidGuidMap.GetCacheForMailbox(context, context.LockedMailboxState);
			FolderHierarchy folderHierarchy = FolderHierarchy.GetFolderHierarchy(context, this, ExchangeShortId.Zero, FolderInformationType.Basic);
			HashSet<ExchangeId> foldersToDelete = new HashSet<ExchangeId>();
			folderHierarchy.ForEachFolderInformation(context, delegate(IFolderInformation folderInformation)
			{
				if (!folderInformation.IsSearchFolder)
				{
					return;
				}
				MailboxState mailboxState = MailboxStateCache.Get(context, folderInformation.MailboxNumber);
				if (mailboxState == null || mailboxState.IsDeleted)
				{
					return;
				}
				IMailboxContext mailboxContext = context.GetMailboxContext(folderInformation.MailboxNumber);
				if (((Mailbox)mailboxContext).IsDead)
				{
					return;
				}
				ExchangeId exchangeId = ExchangeId.CreateFromInternalShortId(context, replidGuidMap, folderInformation.Fid);
				SearchFolder searchFolder2 = (SearchFolder)Folder.OpenFolder(context, this, exchangeId);
				byte[] array;
				IList<ExchangeId> list;
				SearchState searchState;
				searchFolder2.GetSearchCriteria(context, GetSearchCriteriaFlags.FolderIds, out array, out list, out searchState);
				foreach (ExchangeId exchangeId2 in list)
				{
					IFolderInformation folderInformation2 = folderHierarchy.Find(context, exchangeId2.ToExchangeShortId());
					if (folderInformation.MailboxNumber == this.MailboxNumber && folderInformation2.MailboxNumber != this.MailboxNumber)
					{
						foldersToDelete.Add(exchangeId);
					}
					else if (folderInformation.MailboxNumber != this.MailboxNumber && folderInformation2.MailboxNumber == this.MailboxNumber)
					{
						foldersToDelete.Add(exchangeId);
					}
				}
			});
			foreach (ExchangeId id in foldersToDelete)
			{
				SearchFolder searchFolder = (SearchFolder)Folder.OpenFolder(context, this, id);
				searchFolder.Delete(context);
			}
		}
	}
}
