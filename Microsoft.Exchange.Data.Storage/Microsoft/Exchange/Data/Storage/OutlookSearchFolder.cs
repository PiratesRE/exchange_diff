using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutlookSearchFolder : SearchFolder
	{
		internal OutlookSearchFolder(CoreFolder coreFolder) : base(coreFolder)
		{
			base.CoreFolder.QueryExecutor.OnContentsTableAccessed += this.OnContentsTableAccessed;
		}

		private static VersionedId FindAssociatedMessageId(MailboxSession session, Guid clsIdGuid)
		{
			byte[] array = clsIdGuid.ToByteArray();
			VersionedId result;
			using (Folder folder = Folder.Bind(session, DefaultFolderType.CommonViews))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.Associated, null, OutlookSearchFolder.SortByForAssociatedMessage, new PropertyDefinition[]
				{
					ItemSchema.Id,
					InternalSchema.AssociatedSearchFolderId,
					InternalSchema.ItemClass
				}))
				{
					QueryFilter seekFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.AssociatedSearchFolderId, array);
					if (!queryResult.SeekToCondition(SeekReference.OriginBeginning, seekFilter))
					{
						result = null;
					}
					else
					{
						seekFilter = new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ItemClass, "IPM.Microsoft.WunderBar.SFInfo");
						if (!queryResult.SeekToCondition(SeekReference.OriginCurrent, seekFilter))
						{
							result = null;
						}
						else
						{
							object[][] rows = queryResult.GetRows(2);
							if (rows.Length >= 1 && ArrayComparer<byte>.Comparer.Equals(array, rows[0][1] as byte[]))
							{
								VersionedId versionedId = Microsoft.Exchange.Data.Storage.PropertyBag.CheckPropertyValue<VersionedId>(ItemSchema.Id, rows[0][0]);
								if (rows.Length > 1)
								{
									string value = rows[1][2] as string;
									if (!string.IsNullOrEmpty(value) && ArrayComparer<byte>.Comparer.Equals(array, rows[1][1] as byte[]) && "IPM.Microsoft.WunderBar.SFInfo".Equals(value, StringComparison.OrdinalIgnoreCase))
									{
										ExTraceGlobals.StorageTracer.Information(0L, "OutlookSearchFolder::FindAssociatedMessageId. Found more than one associated message for the search folder. Only the first associated message will be used.");
									}
								}
								result = versionedId;
							}
							else
							{
								result = null;
							}
						}
					}
				}
			}
			return result;
		}

		private static int ConvertUtcDateTimeToRTime(ExDateTime dateTime)
		{
			return Convert.ToInt32((dateTime - Util.Date1601Utc).TotalMinutes);
		}

		private static void WriteOutlookSearchFolderDefinitionBlob(MessageItem message, Restriction restriction, bool deepTraversal, StoreId[] folderScope)
		{
			using (Stream stream = message.OpenPropertyStream(InternalSchema.AssociatedSearchFolderDefinition, PropertyOpenMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(stream, Encoding.Unicode))
				{
					Charset itemWindowsCharset = ConvertUtils.GetItemWindowsCharset(message);
					OutlookBlobWriter outlookBlobWriter = new OutlookBlobWriter(binaryWriter, itemWindowsCharset.GetEncoding());
					outlookBlobWriter.WriteInt(4100);
					outlookBlobWriter.WriteInt(72);
					outlookBlobWriter.WriteInt(0);
					outlookBlobWriter.WriteString(string.Empty);
					outlookBlobWriter.WriteSkipBlock();
					outlookBlobWriter.WriteBool(deepTraversal);
					outlookBlobWriter.WriteString(string.Empty);
					outlookBlobWriter.WriteEntryIdList(folderScope);
					outlookBlobWriter.WriteSkipBlock();
					outlookBlobWriter.WriteRestriction(restriction);
					outlookBlobWriter.WriteSkipBlock();
				}
			}
		}

		private static Restriction ReadOutlookSearchFolderDefinitionBlob(MessageItem message, out bool deepTraversal, out StoreId[] ids)
		{
			Restriction result = null;
			try
			{
				using (Stream stream = message.OpenPropertyStream(InternalSchema.AssociatedSearchFolderDefinition, PropertyOpenMode.ReadOnly))
				{
					using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.Unicode))
					{
						Charset itemWindowsCharset = ConvertUtils.GetItemWindowsCharset(message);
						OutlookBlobReader outlookBlobReader = new OutlookBlobReader(binaryReader, itemWindowsCharset.GetEncoding());
						int num = outlookBlobReader.ReadInt();
						if (num != 4100)
						{
							throw new CorruptDataException(ServerStrings.ExSearchFolderCorruptOutlookBlob("version"));
						}
						int num2 = outlookBlobReader.ReadInt();
						if (num2 != 72)
						{
							throw new CorruptDataException(ServerStrings.ExSearchFolderCorruptOutlookBlob("storage type"));
						}
						outlookBlobReader.ReadInt();
						outlookBlobReader.ReadString();
						outlookBlobReader.ReadSkipBlock();
						deepTraversal = outlookBlobReader.ReadBool();
						outlookBlobReader.ReadString();
						ids = outlookBlobReader.ReadEntryIdList();
						outlookBlobReader.ReadSkipBlock();
						result = outlookBlobReader.ReadRestriction();
					}
				}
			}
			catch (EndOfStreamException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExSearchFolderCorruptOutlookBlob("EndOfStreamException"), innerException);
			}
			catch (OutOfMemoryException innerException2)
			{
				throw new CorruptDataException(ServerStrings.ExSearchFolderCorruptOutlookBlob("OutOfMemoryException"), innerException2);
			}
			return result;
		}

		public new static OutlookSearchFolder Create(StoreSession session, StoreId parentFolderId)
		{
			throw new NotSupportedException();
		}

		public static StoreObjectId Recreate(MailboxSession session, Guid searchFolderClsId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (searchFolderClsId == Guid.Empty)
			{
				throw new ArgumentException("Guid is empty", "searchFolderClsId");
			}
			using (Folder folder = Folder.Bind(session, DefaultFolderType.SearchFolders))
			{
				using (QueryResult queryResult = folder.FolderQuery(FolderQueryFlags.None, null, null, new PropertyDefinition[]
				{
					InternalSchema.OutlookSearchFolderClsId
				}))
				{
					for (;;)
					{
						object[][] rows = queryResult.GetRows(10000);
						for (int i = 0; i < rows.Length; i++)
						{
							if (rows[i][0] is Guid && ((Guid)rows[i][0]).Equals(searchFolderClsId))
							{
								goto Block_9;
							}
						}
						if (rows.Length <= 0)
						{
							goto Block_11;
						}
					}
					Block_9:
					throw new ObjectExistedException(ServerStrings.ExSearchFolderAlreadyExists(searchFolderClsId));
					Block_11:;
				}
			}
			VersionedId versionedId = OutlookSearchFolder.FindAssociatedMessageId(session, searchFolderClsId);
			if (versionedId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExSearchFolderNoAssociatedItem(searchFolderClsId));
			}
			StoreObjectId objectId;
			using (MessageItem messageItem = MessageItem.Bind(session, versionedId))
			{
				bool deepTraversal;
				StoreId[] folderScope;
				Restriction restriction = OutlookSearchFolder.ReadOutlookSearchFolderDefinitionBlob(messageItem, out deepTraversal, out folderScope);
				QueryFilter searchQuery = FilterRestrictionConverter.CreateFilter(session, session.Mailbox.MapiStore, restriction, true);
				SearchFolderCriteria searchFolderCriteria = new SearchFolderCriteria(searchQuery, folderScope);
				searchFolderCriteria.DeepTraversal = deepTraversal;
				string valueOrDefault = messageItem.GetValueOrDefault<string>(InternalSchema.DisplayName, string.Empty);
				using (OutlookSearchFolder outlookSearchFolder = OutlookSearchFolder.Create(session, valueOrDefault))
				{
					outlookSearchFolder[InternalSchema.OutlookSearchFolderClsId] = searchFolderClsId;
					FolderSaveResult folderSaveResult = outlookSearchFolder.Save();
					if (folderSaveResult.OperationResult != OperationResult.Succeeded)
					{
						throw folderSaveResult.ToException(ServerStrings.ExCannotCreateFolder(folderSaveResult));
					}
					outlookSearchFolder.Load(null);
					outlookSearchFolder.ApplyContinuousSearch(searchFolderCriteria);
					objectId = outlookSearchFolder.Id.ObjectId;
				}
			}
			return objectId;
		}

		public new static OutlookSearchFolder Create(StoreSession session, StoreId parentFolderId, string displayName, CreateMode createMode)
		{
			throw new NotSupportedException();
		}

		public static OutlookSearchFolder Create(MailboxSession session, string displayName)
		{
			StoreObjectId defaultFolderId = session.GetDefaultFolderId(DefaultFolderType.SearchFolders);
			if (defaultFolderId == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ExDefaultFolderNotFound(DefaultFolderType.SearchFolders));
			}
			OutlookSearchFolder outlookSearchFolder = (OutlookSearchFolder)Folder.Create(session, defaultFolderId, StoreObjectType.OutlookSearchFolder, displayName, CreateMode.CreateNew);
			outlookSearchFolder[InternalSchema.OutlookSearchFolderClsId] = Guid.NewGuid();
			return outlookSearchFolder;
		}

		public new static OutlookSearchFolder Bind(StoreSession session, StoreId folderId)
		{
			return OutlookSearchFolder.Bind(session, folderId, null);
		}

		public new static OutlookSearchFolder Bind(StoreSession session, StoreId folderId, ICollection<PropertyDefinition> propsToReturn)
		{
			propsToReturn = InternalSchema.Combine<PropertyDefinition>(FolderSchema.Instance.AutoloadProperties, propsToReturn);
			return Folder.InternalBind<OutlookSearchFolder>(session, folderId, propsToReturn);
		}

		public new static OutlookSearchFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType)
		{
			return OutlookSearchFolder.Bind(session, defaultFolderType, null);
		}

		public new static OutlookSearchFolder Bind(MailboxSession session, DefaultFolderType defaultFolderType, ICollection<PropertyDefinition> propsToReturn)
		{
			EnumValidator.ThrowIfInvalid<DefaultFolderType>(defaultFolderType, "defaultFolderType");
			DefaultFolder defaultFolder = session.InternalGetDefaultFolder(defaultFolderType);
			if (defaultFolder.StoreObjectType != StoreObjectType.OutlookSearchFolder)
			{
				throw new ArgumentOutOfRangeException("defaultFolderType");
			}
			return OutlookSearchFolder.Bind(session, session.SafeGetDefaultFolderId(defaultFolderType), propsToReturn);
		}

		public bool TryGetQueryFilter(out QueryFilter filter)
		{
			MailboxSession mailboxSession = base.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new InvalidOperationException(ServerStrings.ExOutlookSearchFolderDoesNotHaveMailboxSession);
			}
			VersionedId versionedId = OutlookSearchFolder.FindAssociatedMessageId(mailboxSession, (Guid)this[InternalSchema.OutlookSearchFolderClsId]);
			if (versionedId == null)
			{
				filter = null;
				return false;
			}
			bool result;
			using (MessageItem messageItem = MessageItem.Bind(mailboxSession, versionedId))
			{
				bool flag;
				StoreId[] array;
				Restriction restriction = OutlookSearchFolder.ReadOutlookSearchFolderDefinitionBlob(messageItem, out flag, out array);
				filter = FilterRestrictionConverter.CreateFilter(mailboxSession, mailboxSession.Mailbox.MapiStore, restriction, true);
				result = true;
			}
			return result;
		}

		public void MakeVisibleToOutlook()
		{
			this.MakeVisibleToOutlook(false);
		}

		public void MakeVisibleToOutlook(bool replaceAssociatedMessageIfPresent)
		{
			this.MakeVisibleToOutlook(replaceAssociatedMessageIfPresent, null);
		}

		public void MakeVisibleToOutlook(bool replaceAssociatedMessageIfPresent, SearchFolderCriteria criteria)
		{
			if (this.IsDirty)
			{
				throw new InvalidOperationException(ServerStrings.ExMustSaveFolderToMakeVisibleToOutlook);
			}
			VersionedId versionedId = OutlookSearchFolder.FindAssociatedMessageId((MailboxSession)base.Session, (Guid)this[InternalSchema.OutlookSearchFolderClsId]);
			if (versionedId != null && !replaceAssociatedMessageIfPresent)
			{
				throw new ObjectExistedException(ServerStrings.ExSearchFolderIsAlreadyVisibleToOutlook);
			}
			if (criteria == null)
			{
				try
				{
					criteria = base.GetSearchCriteria();
					goto IL_70;
				}
				catch (ObjectNotInitializedException innerException)
				{
					throw new InvalidOperationException(ServerStrings.ExMustSetSearchCriteriaToMakeVisibleToOutlook, innerException);
				}
			}
			base.ApplyContinuousSearch(criteria);
			IL_70:
			this.CreateOrHijackAssociatedMessage(versionedId, criteria);
		}

		public static AggregateOperationResult DeleteOutlookSearchFolder(DeleteItemFlags deleteItemFlags, MailboxSession session, StoreId outlookSearchFolderId)
		{
			EnumValidator.ThrowIfInvalid<DeleteItemFlags>(deleteItemFlags);
			VersionedId versionedId;
			using (OutlookSearchFolder outlookSearchFolder = OutlookSearchFolder.Bind(session, outlookSearchFolderId))
			{
				versionedId = OutlookSearchFolder.FindAssociatedMessageId((MailboxSession)outlookSearchFolder.Session, (Guid)outlookSearchFolder[InternalSchema.OutlookSearchFolderClsId]);
			}
			StoreId[] ids;
			if (versionedId != null)
			{
				ids = new StoreId[]
				{
					outlookSearchFolderId,
					versionedId
				};
			}
			else
			{
				ids = new StoreId[]
				{
					outlookSearchFolderId
				};
			}
			return session.Delete(deleteItemFlags, ids);
		}

		private void OnContentsTableAccessed()
		{
			this.CheckDisposed("OnContentsTableAccessed");
			try
			{
				this.UpdateAssociatedSearchFolderLastUsedTime();
			}
			catch (StoragePermanentException arg)
			{
				ExTraceGlobals.StorageTracer.Information<string, StoragePermanentException>((long)this.GetHashCode(), "OutlookSearchFolder::OnContentsTableAccessed. Failed to update the last used time of the search folder in its associated message. SearchFolder DisplayName = {0}. Exception = {1}.", base.DisplayName, arg);
			}
			catch (StorageTransientException arg2)
			{
				ExTraceGlobals.StorageTracer.Information<string, StorageTransientException>((long)this.GetHashCode(), "OutlookSearchFolder::OnContentsTableAccessed. Failed to update the last used time of the search folder in its associated message. SearchFolder DisplayName = {0}. Exception = {1}.", base.DisplayName, arg2);
			}
		}

		private void UpdateAssociatedSearchFolderLastUsedTime()
		{
			VersionedId versionedId = OutlookSearchFolder.FindAssociatedMessageId((MailboxSession)base.Session, (Guid)this[InternalSchema.OutlookSearchFolderClsId]);
			if (versionedId != null)
			{
				using (MessageItem messageItem = MessageItem.Bind(base.Session, versionedId))
				{
					int num = OutlookSearchFolder.ConvertUtcDateTimeToRTime(ExDateTime.UtcNow);
					messageItem[MessageItemSchema.AssociatedSearchFolderLastUsedTime] = num;
					messageItem.Save(SaveMode.NoConflictResolution);
					return;
				}
			}
			ExTraceGlobals.StorageTracer.Information<string>((long)this.GetHashCode(), "OutlookSearchFolder::UpdateAssociatedSearchFolderLastUsedTime. Failed to update the last used time of the search folder in its associated message. Associated message not found. SearchFolder DisplayName = {0}.", base.DisplayName);
		}

		private void CreateOrHijackAssociatedMessage(VersionedId associatedMessageId, SearchFolderCriteria criteria)
		{
			MailboxSession mailboxSession = (MailboxSession)base.Session;
			MessageItem messageItem = null;
			try
			{
				if (associatedMessageId == null)
				{
					messageItem = MessageItem.CreateAssociated(base.Session, mailboxSession.SafeGetDefaultFolderId(DefaultFolderType.CommonViews));
				}
				else
				{
					messageItem = MessageItem.Bind(base.Session, associatedMessageId);
				}
				messageItem[InternalSchema.ItemClass] = "IPM.Microsoft.WunderBar.SFInfo";
				messageItem[InternalSchema.AssociatedSearchFolderId] = ((Guid)this[InternalSchema.OutlookSearchFolderClsId]).ToByteArray();
				messageItem[InternalSchema.DisplayName] = this[FolderSchema.DisplayName];
				messageItem[ItemSchema.Subject] = this[FolderSchema.DisplayName];
				ExtendedFolderFlags? valueAsNullable = base.GetValueAsNullable<ExtendedFolderFlags>(FolderSchema.ExtendedFolderFlags);
				if (valueAsNullable != null)
				{
					messageItem[InternalSchema.AssociatedSearchFolderFlags] = valueAsNullable.Value;
				}
				int num = OutlookSearchFolder.ConvertUtcDateTimeToRTime(ExDateTime.UtcNow);
				messageItem[InternalSchema.AssociatedSearchFolderLastUsedTime] = num;
				messageItem[InternalSchema.AssociatedSearchFolderExpiration] = num;
				messageItem[InternalSchema.AssociatedSearchFolderTemplateId] = 1;
				messageItem[InternalSchema.AssociatedSearchFolderTag] = 0;
				this.WriteOutlookSearchFolderDefinitionBlob(messageItem, criteria);
				messageItem[InternalSchema.AssociatedSearchFolderStorageType] = 72;
				messageItem.Save(SaveMode.FailOnAnyConflict);
			}
			finally
			{
				if (messageItem != null)
				{
					messageItem.Dispose();
					messageItem = null;
				}
			}
		}

		private void WriteOutlookSearchFolderDefinitionBlob(MessageItem message, SearchFolderCriteria criteria)
		{
			Restriction restriction = FilterRestrictionConverter.CreateRestriction(base.Session, base.PropertyBag.ExTimeZone, base.MapiProp, criteria.SearchQuery);
			OutlookSearchFolder.WriteOutlookSearchFolderDefinitionBlob(message, restriction, criteria.DeepTraversal, criteria.FolderScope);
		}

		private const int CustomOutlookSearchFolderTemplateId = 1;

		private const int OutlookStorageType = 72;

		private const string OutlookSearchFolderAssociatedMessageClass = "IPM.Microsoft.WunderBar.SFInfo";

		private const int OutlookSearchFolderDataVersion = 4100;

		private static readonly SortBy[] SortByForAssociatedMessage = new SortBy[]
		{
			new SortBy(InternalSchema.AssociatedSearchFolderId, SortOrder.Ascending),
			new SortBy(StoreObjectSchema.ItemClass, SortOrder.Ascending)
		};

		internal static class TestAccess
		{
			internal static void WriteOutlookSearchFolderDefinitionBlob(MessageItem message, Restriction restriction, bool deepTraversal, StoreId[] folderScope)
			{
				OutlookSearchFolder.WriteOutlookSearchFolderDefinitionBlob(message, restriction, deepTraversal, folderScope);
			}

			internal static Restriction ReadOutlookSearchFolderDefinitionBlob(MessageItem message, out bool deepTraversal, out StoreId[] ids)
			{
				return OutlookSearchFolder.ReadOutlookSearchFolderDefinitionBlob(message, out deepTraversal, out ids);
			}
		}
	}
}
