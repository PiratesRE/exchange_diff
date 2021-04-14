using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class FolderTable
	{
		internal FolderTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.lcnCurrent = Factory.CreatePhysicalColumn("LcnCurrent", "LcnCurrent", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.versionHistory = Factory.CreatePhysicalColumn("VersionHistory", "VersionHistory", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1048576, 0, 1048576);
			this.parentFolderId = Factory.CreatePhysicalColumn("ParentFolderId", "ParentFolderId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.creationTime = Factory.CreatePhysicalColumn("CreationTime", "CreationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.creatorSid = Factory.CreatePhysicalColumn("CreatorSid", "CreatorSid", typeof(byte[]), false, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.lastModificationTime = Factory.CreatePhysicalColumn("LastModificationTime", "LastModificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.sourceKey = Factory.CreatePhysicalColumn("SourceKey", "SourceKey", typeof(byte[]), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.changeKey = Factory.CreatePhysicalColumn("ChangeKey", "ChangeKey", typeof(byte[]), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.localCommitTimeMax = Factory.CreatePhysicalColumn("LocalCommitTimeMax", "LocalCommitTimeMax", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastModifierSid = Factory.CreatePhysicalColumn("LastModifierSid", "LastModifierSid", typeof(byte[]), false, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.displayName = Factory.CreatePhysicalColumn("DisplayName", "DisplayName", typeof(string), true, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.comment = Factory.CreatePhysicalColumn("Comment", "Comment", typeof(string), true, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.containerClass = Factory.CreatePhysicalColumn("ContainerClass", "ContainerClass", typeof(string), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.messageCount = Factory.CreatePhysicalColumn("MessageCount", "MessageCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.totalDeletedCount = Factory.CreatePhysicalColumn("TotalDeletedCount", "TotalDeletedCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.unreadMessageCount = Factory.CreatePhysicalColumn("UnreadMessageCount", "UnreadMessageCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.virtualUnreadMessageCount = Factory.CreatePhysicalColumn("VirtualUnreadMessageCount", "VirtualUnreadMessageCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageSize = Factory.CreatePhysicalColumn("MessageSize", "MessageSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageAttachCount = Factory.CreatePhysicalColumn("MessageAttachCount", "MessageAttachCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageHasAttachCount = Factory.CreatePhysicalColumn("MessageHasAttachCount", "MessageHasAttachCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenItemCount = Factory.CreatePhysicalColumn("HiddenItemCount", "HiddenItemCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.unreadHiddenItemCount = Factory.CreatePhysicalColumn("UnreadHiddenItemCount", "UnreadHiddenItemCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenItemSize = Factory.CreatePhysicalColumn("HiddenItemSize", "HiddenItemSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenItemHasAttachCount = Factory.CreatePhysicalColumn("HiddenItemHasAttachCount", "HiddenItemHasAttachCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenItemAttachCount = Factory.CreatePhysicalColumn("HiddenItemAttachCount", "HiddenItemAttachCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.displayType = Factory.CreatePhysicalColumn("DisplayType", "DisplayType", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.conversationCount = Factory.CreatePhysicalColumn("ConversationCount", "ConversationCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.folderCount = Factory.CreatePhysicalColumn("FolderCount", "FolderCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.specialFolderNumber = Factory.CreatePhysicalColumn("SpecialFolderNumber", "SpecialFolderNumber", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.reservedMessageIdGlobCntCurrent = Factory.CreatePhysicalColumn("ReservedMessageIdGlobCntCurrent", "ReservedMessageIdGlobCntCurrent", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.reservedMessageIdGlobCntMax = Factory.CreatePhysicalColumn("ReservedMessageIdGlobCntMax", "ReservedMessageIdGlobCntMax", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.reservedMessageCnGlobCntCurrent = Factory.CreatePhysicalColumn("ReservedMessageCnGlobCntCurrent", "ReservedMessageCnGlobCntCurrent", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.reservedMessageCnGlobCntMax = Factory.CreatePhysicalColumn("ReservedMessageCnGlobCntMax", "ReservedMessageCnGlobCntMax", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.nextArticleNumber = Factory.CreatePhysicalColumn("NextArticleNumber", "NextArticleNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.midsetDeleted = Factory.CreatePhysicalColumn("MidsetDeleted", "MidsetDeleted", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.normalItemPromotedColumns = Factory.CreatePhysicalColumn("NormalItemPromotedColumns", "NormalItemPromotedColumns", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.hiddenItemPromotedColumns = Factory.CreatePhysicalColumn("HiddenItemPromotedColumns", "HiddenItemPromotedColumns", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.promotionTimestamp = Factory.CreatePhysicalColumn("PromotionTimestamp", "PromotionTimestamp", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.promotionUseHistory = Factory.CreatePhysicalColumn("PromotionUseHistory", "PromotionUseHistory", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.queryCriteria = Factory.CreatePhysicalColumn("QueryCriteria", "QueryCriteria", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.searchState = Factory.CreatePhysicalColumn("SearchState", "SearchState", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.setSearchCriteriaFlags = Factory.CreatePhysicalColumn("SetSearchCriteriaFlags", "SetSearchCriteriaFlags", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.scopeFolders = Factory.CreatePhysicalColumn("ScopeFolders", "ScopeFolders", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.nonRecursiveSearchBacklinks = Factory.CreatePhysicalColumn("NonRecursiveSearchBacklinks", "NonRecursiveSearchBacklinks", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.recursiveSearchBacklinks = Factory.CreatePhysicalColumn("RecursiveSearchBacklinks", "RecursiveSearchBacklinks", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.logicalIndexNumber = Factory.CreatePhysicalColumn("LogicalIndexNumber", "LogicalIndexNumber", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.largePropertyValueBlob = Factory.CreatePhysicalColumn("LargePropertyValueBlob", "LargePropertyValueBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.propertyBag = Factory.CreatePhysicalColumn("PropertyBag", "PropertyBag", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 4, 0, 4);
			this.aclTableAndSecurityDescriptor = Factory.CreatePhysicalColumn("AclTableAndSecurityDescriptor", "AclTableAndSecurityDescriptor", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.midsetDeletedDelta = Factory.CreatePhysicalColumn("MidsetDeletedDelta", "MidsetDeletedDelta", typeof(byte[]), true, false, false, false, true, Visibility.Public, 512, 0, 512);
			this.reservedDocumentIdCurrent = Factory.CreatePhysicalColumn("ReservedDocumentIdCurrent", "ReservedDocumentIdCurrent", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.reservedDocumentIdMax = Factory.CreatePhysicalColumn("ReservedDocumentIdMax", "ReservedDocumentIdMax", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string name = "FolderPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.folderPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId
			});
			string name2 = "FolderChangeNumber";
			bool primaryKey2 = false;
			bool unique2 = true;
			bool schemaExtension2 = false;
			bool[] conditional2 = new bool[3];
			this.folderChangeNumberIndex = new Index(name2, primaryKey2, unique2, schemaExtension2, conditional2, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.LcnCurrent,
				this.FolderId
			});
			string name3 = "FolderByParent";
			bool primaryKey3 = false;
			bool unique3 = true;
			bool schemaExtension3 = false;
			bool[] conditional3 = new bool[3];
			this.folderByParentIndex = new Index(name3, primaryKey3, unique3, schemaExtension3, conditional3, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.ParentFolderId,
				this.FolderId
			});
			Index[] indexes = new Index[]
			{
				this.FolderPK,
				this.FolderChangeNumberIndex,
				this.FolderByParentIndex
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, null, this.PropertyBag, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[]
			{
				this.VirtualUnreadMessageCount,
				this.PropertyBag
			};
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.LcnCurrent,
				this.VersionHistory,
				this.ParentFolderId,
				this.CreationTime,
				this.CreatorSid,
				this.LastModificationTime,
				this.SourceKey,
				this.ChangeKey,
				this.LocalCommitTimeMax,
				this.LastModifierSid,
				this.DisplayName,
				this.Comment,
				this.ContainerClass,
				this.MessageCount,
				this.TotalDeletedCount,
				this.UnreadMessageCount,
				this.MessageSize,
				this.MessageAttachCount,
				this.MessageHasAttachCount,
				this.HiddenItemCount,
				this.UnreadHiddenItemCount,
				this.HiddenItemSize,
				this.HiddenItemHasAttachCount,
				this.HiddenItemAttachCount,
				this.DisplayType,
				this.ConversationCount,
				this.FolderCount,
				this.SpecialFolderNumber,
				this.ReservedMessageIdGlobCntCurrent,
				this.ReservedMessageIdGlobCntMax,
				this.ReservedMessageCnGlobCntCurrent,
				this.ReservedMessageCnGlobCntMax,
				this.NextArticleNumber,
				this.MidsetDeleted,
				this.NormalItemPromotedColumns,
				this.HiddenItemPromotedColumns,
				this.PromotionTimestamp,
				this.PromotionUseHistory,
				this.QueryCriteria,
				this.SearchState,
				this.SetSearchCriteriaFlags,
				this.ScopeFolders,
				this.NonRecursiveSearchBacklinks,
				this.RecursiveSearchBacklinks,
				this.LogicalIndexNumber,
				this.PropertyBlob,
				this.LargePropertyValueBlob,
				this.ExtensionBlob,
				this.AclTableAndSecurityDescriptor,
				this.MidsetDeletedDelta,
				this.ReservedDocumentIdCurrent,
				this.ReservedDocumentIdMax,
				this.MailboxNumber
			};
			this.table = Factory.CreateTable("Folder", TableClass.Folder, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Redacted, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public PhysicalColumn FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public PhysicalColumn LcnCurrent
		{
			get
			{
				return this.lcnCurrent;
			}
		}

		public PhysicalColumn VersionHistory
		{
			get
			{
				return this.versionHistory;
			}
		}

		public PhysicalColumn ParentFolderId
		{
			get
			{
				return this.parentFolderId;
			}
		}

		public PhysicalColumn CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public PhysicalColumn CreatorSid
		{
			get
			{
				return this.creatorSid;
			}
		}

		public PhysicalColumn LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public PhysicalColumn SourceKey
		{
			get
			{
				return this.sourceKey;
			}
		}

		public PhysicalColumn ChangeKey
		{
			get
			{
				return this.changeKey;
			}
		}

		public PhysicalColumn LocalCommitTimeMax
		{
			get
			{
				return this.localCommitTimeMax;
			}
		}

		public PhysicalColumn LastModifierSid
		{
			get
			{
				return this.lastModifierSid;
			}
		}

		public PhysicalColumn DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public PhysicalColumn Comment
		{
			get
			{
				return this.comment;
			}
		}

		public PhysicalColumn ContainerClass
		{
			get
			{
				return this.containerClass;
			}
		}

		public PhysicalColumn MessageCount
		{
			get
			{
				return this.messageCount;
			}
		}

		public PhysicalColumn TotalDeletedCount
		{
			get
			{
				return this.totalDeletedCount;
			}
		}

		public PhysicalColumn UnreadMessageCount
		{
			get
			{
				return this.unreadMessageCount;
			}
		}

		public PhysicalColumn VirtualUnreadMessageCount
		{
			get
			{
				return this.virtualUnreadMessageCount;
			}
		}

		public PhysicalColumn MessageSize
		{
			get
			{
				return this.messageSize;
			}
		}

		public PhysicalColumn MessageAttachCount
		{
			get
			{
				return this.messageAttachCount;
			}
		}

		public PhysicalColumn MessageHasAttachCount
		{
			get
			{
				return this.messageHasAttachCount;
			}
		}

		public PhysicalColumn HiddenItemCount
		{
			get
			{
				return this.hiddenItemCount;
			}
		}

		public PhysicalColumn UnreadHiddenItemCount
		{
			get
			{
				return this.unreadHiddenItemCount;
			}
		}

		public PhysicalColumn HiddenItemSize
		{
			get
			{
				return this.hiddenItemSize;
			}
		}

		public PhysicalColumn HiddenItemHasAttachCount
		{
			get
			{
				return this.hiddenItemHasAttachCount;
			}
		}

		public PhysicalColumn HiddenItemAttachCount
		{
			get
			{
				return this.hiddenItemAttachCount;
			}
		}

		public PhysicalColumn DisplayType
		{
			get
			{
				return this.displayType;
			}
		}

		public PhysicalColumn ConversationCount
		{
			get
			{
				return this.conversationCount;
			}
		}

		public PhysicalColumn FolderCount
		{
			get
			{
				return this.folderCount;
			}
		}

		public PhysicalColumn SpecialFolderNumber
		{
			get
			{
				return this.specialFolderNumber;
			}
		}

		public PhysicalColumn ReservedMessageIdGlobCntCurrent
		{
			get
			{
				return this.reservedMessageIdGlobCntCurrent;
			}
		}

		public PhysicalColumn ReservedMessageIdGlobCntMax
		{
			get
			{
				return this.reservedMessageIdGlobCntMax;
			}
		}

		public PhysicalColumn ReservedMessageCnGlobCntCurrent
		{
			get
			{
				return this.reservedMessageCnGlobCntCurrent;
			}
		}

		public PhysicalColumn ReservedMessageCnGlobCntMax
		{
			get
			{
				return this.reservedMessageCnGlobCntMax;
			}
		}

		public PhysicalColumn NextArticleNumber
		{
			get
			{
				return this.nextArticleNumber;
			}
		}

		public PhysicalColumn MidsetDeleted
		{
			get
			{
				return this.midsetDeleted;
			}
		}

		public PhysicalColumn NormalItemPromotedColumns
		{
			get
			{
				return this.normalItemPromotedColumns;
			}
		}

		public PhysicalColumn HiddenItemPromotedColumns
		{
			get
			{
				return this.hiddenItemPromotedColumns;
			}
		}

		public PhysicalColumn PromotionTimestamp
		{
			get
			{
				return this.promotionTimestamp;
			}
		}

		public PhysicalColumn PromotionUseHistory
		{
			get
			{
				return this.promotionUseHistory;
			}
		}

		public PhysicalColumn QueryCriteria
		{
			get
			{
				return this.queryCriteria;
			}
		}

		public PhysicalColumn SearchState
		{
			get
			{
				return this.searchState;
			}
		}

		public PhysicalColumn SetSearchCriteriaFlags
		{
			get
			{
				return this.setSearchCriteriaFlags;
			}
		}

		public PhysicalColumn ScopeFolders
		{
			get
			{
				return this.scopeFolders;
			}
		}

		public PhysicalColumn NonRecursiveSearchBacklinks
		{
			get
			{
				return this.nonRecursiveSearchBacklinks;
			}
		}

		public PhysicalColumn RecursiveSearchBacklinks
		{
			get
			{
				return this.recursiveSearchBacklinks;
			}
		}

		public PhysicalColumn LogicalIndexNumber
		{
			get
			{
				return this.logicalIndexNumber;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn LargePropertyValueBlob
		{
			get
			{
				return this.largePropertyValueBlob;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public PhysicalColumn AclTableAndSecurityDescriptor
		{
			get
			{
				return this.aclTableAndSecurityDescriptor;
			}
		}

		public PhysicalColumn MidsetDeletedDelta
		{
			get
			{
				return this.midsetDeletedDelta;
			}
		}

		public PhysicalColumn ReservedDocumentIdCurrent
		{
			get
			{
				return this.reservedDocumentIdCurrent;
			}
		}

		public PhysicalColumn ReservedDocumentIdMax
		{
			get
			{
				return this.reservedDocumentIdMax;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Index FolderPK
		{
			get
			{
				return this.folderPK;
			}
		}

		public Index FolderChangeNumberIndex
		{
			get
			{
				return this.folderChangeNumberIndex;
			}
		}

		public Index FolderByParentIndex
		{
			get
			{
				return this.folderByParentIndex;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.mailboxPartitionNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxPartitionNumber = null;
			}
			physicalColumn = this.folderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.folderId = null;
			}
			physicalColumn = this.lcnCurrent;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lcnCurrent = null;
			}
			physicalColumn = this.versionHistory;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.versionHistory = null;
			}
			physicalColumn = this.parentFolderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.parentFolderId = null;
			}
			physicalColumn = this.creationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.creationTime = null;
			}
			physicalColumn = this.creatorSid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.creatorSid = null;
			}
			physicalColumn = this.lastModificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModificationTime = null;
			}
			physicalColumn = this.sourceKey;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.sourceKey = null;
			}
			physicalColumn = this.changeKey;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.changeKey = null;
			}
			physicalColumn = this.localCommitTimeMax;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.localCommitTimeMax = null;
			}
			physicalColumn = this.lastModifierSid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModifierSid = null;
			}
			physicalColumn = this.displayName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.displayName = null;
			}
			physicalColumn = this.comment;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.comment = null;
			}
			physicalColumn = this.containerClass;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.containerClass = null;
			}
			physicalColumn = this.messageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageCount = null;
			}
			physicalColumn = this.totalDeletedCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.totalDeletedCount = null;
			}
			physicalColumn = this.unreadMessageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.unreadMessageCount = null;
			}
			physicalColumn = this.virtualUnreadMessageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.virtualUnreadMessageCount = null;
			}
			physicalColumn = this.messageSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageSize = null;
			}
			physicalColumn = this.messageAttachCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageAttachCount = null;
			}
			physicalColumn = this.messageHasAttachCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageHasAttachCount = null;
			}
			physicalColumn = this.hiddenItemCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenItemCount = null;
			}
			physicalColumn = this.unreadHiddenItemCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.unreadHiddenItemCount = null;
			}
			physicalColumn = this.hiddenItemSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenItemSize = null;
			}
			physicalColumn = this.hiddenItemHasAttachCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenItemHasAttachCount = null;
			}
			physicalColumn = this.hiddenItemAttachCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenItemAttachCount = null;
			}
			physicalColumn = this.displayType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.displayType = null;
			}
			physicalColumn = this.conversationCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationCount = null;
			}
			physicalColumn = this.folderCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.folderCount = null;
			}
			physicalColumn = this.specialFolderNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.specialFolderNumber = null;
			}
			physicalColumn = this.reservedMessageIdGlobCntCurrent;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedMessageIdGlobCntCurrent = null;
			}
			physicalColumn = this.reservedMessageIdGlobCntMax;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedMessageIdGlobCntMax = null;
			}
			physicalColumn = this.reservedMessageCnGlobCntCurrent;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedMessageCnGlobCntCurrent = null;
			}
			physicalColumn = this.reservedMessageCnGlobCntMax;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedMessageCnGlobCntMax = null;
			}
			physicalColumn = this.nextArticleNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.nextArticleNumber = null;
			}
			physicalColumn = this.midsetDeleted;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.midsetDeleted = null;
			}
			physicalColumn = this.normalItemPromotedColumns;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.normalItemPromotedColumns = null;
			}
			physicalColumn = this.hiddenItemPromotedColumns;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenItemPromotedColumns = null;
			}
			physicalColumn = this.promotionTimestamp;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.promotionTimestamp = null;
			}
			physicalColumn = this.promotionUseHistory;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.promotionUseHistory = null;
			}
			physicalColumn = this.queryCriteria;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.queryCriteria = null;
			}
			physicalColumn = this.searchState;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.searchState = null;
			}
			physicalColumn = this.setSearchCriteriaFlags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.setSearchCriteriaFlags = null;
			}
			physicalColumn = this.scopeFolders;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.scopeFolders = null;
			}
			physicalColumn = this.nonRecursiveSearchBacklinks;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.nonRecursiveSearchBacklinks = null;
			}
			physicalColumn = this.recursiveSearchBacklinks;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.recursiveSearchBacklinks = null;
			}
			physicalColumn = this.logicalIndexNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.logicalIndexNumber = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
			}
			physicalColumn = this.largePropertyValueBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.largePropertyValueBlob = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.propertyBag;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBag = null;
			}
			physicalColumn = this.aclTableAndSecurityDescriptor;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.aclTableAndSecurityDescriptor = null;
			}
			physicalColumn = this.midsetDeletedDelta;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.midsetDeletedDelta = null;
			}
			physicalColumn = this.reservedDocumentIdCurrent;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedDocumentIdCurrent = null;
			}
			physicalColumn = this.reservedDocumentIdMax;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.reservedDocumentIdMax = null;
			}
			physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.folderPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.folderPK = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.folderChangeNumberIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.folderChangeNumberIndex = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.folderByParentIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.folderByParentIndex = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string FolderIdName = "FolderId";

		public const string LcnCurrentName = "LcnCurrent";

		public const string VersionHistoryName = "VersionHistory";

		public const string ParentFolderIdName = "ParentFolderId";

		public const string CreationTimeName = "CreationTime";

		public const string CreatorSidName = "CreatorSid";

		public const string LastModificationTimeName = "LastModificationTime";

		public const string SourceKeyName = "SourceKey";

		public const string ChangeKeyName = "ChangeKey";

		public const string LocalCommitTimeMaxName = "LocalCommitTimeMax";

		public const string LastModifierSidName = "LastModifierSid";

		public const string DisplayNameName = "DisplayName";

		public const string CommentName = "Comment";

		public const string ContainerClassName = "ContainerClass";

		public const string MessageCountName = "MessageCount";

		public const string TotalDeletedCountName = "TotalDeletedCount";

		public const string UnreadMessageCountName = "UnreadMessageCount";

		public const string VirtualUnreadMessageCountName = "VirtualUnreadMessageCount";

		public const string MessageSizeName = "MessageSize";

		public const string MessageAttachCountName = "MessageAttachCount";

		public const string MessageHasAttachCountName = "MessageHasAttachCount";

		public const string HiddenItemCountName = "HiddenItemCount";

		public const string UnreadHiddenItemCountName = "UnreadHiddenItemCount";

		public const string HiddenItemSizeName = "HiddenItemSize";

		public const string HiddenItemHasAttachCountName = "HiddenItemHasAttachCount";

		public const string HiddenItemAttachCountName = "HiddenItemAttachCount";

		public const string DisplayTypeName = "DisplayType";

		public const string ConversationCountName = "ConversationCount";

		public const string FolderCountName = "FolderCount";

		public const string SpecialFolderNumberName = "SpecialFolderNumber";

		public const string ReservedMessageIdGlobCntCurrentName = "ReservedMessageIdGlobCntCurrent";

		public const string ReservedMessageIdGlobCntMaxName = "ReservedMessageIdGlobCntMax";

		public const string ReservedMessageCnGlobCntCurrentName = "ReservedMessageCnGlobCntCurrent";

		public const string ReservedMessageCnGlobCntMaxName = "ReservedMessageCnGlobCntMax";

		public const string NextArticleNumberName = "NextArticleNumber";

		public const string MidsetDeletedName = "MidsetDeleted";

		public const string NormalItemPromotedColumnsName = "NormalItemPromotedColumns";

		public const string HiddenItemPromotedColumnsName = "HiddenItemPromotedColumns";

		public const string PromotionTimestampName = "PromotionTimestamp";

		public const string PromotionUseHistoryName = "PromotionUseHistory";

		public const string QueryCriteriaName = "QueryCriteria";

		public const string SearchStateName = "SearchState";

		public const string SetSearchCriteriaFlagsName = "SetSearchCriteriaFlags";

		public const string ScopeFoldersName = "ScopeFolders";

		public const string NonRecursiveSearchBacklinksName = "NonRecursiveSearchBacklinks";

		public const string RecursiveSearchBacklinksName = "RecursiveSearchBacklinks";

		public const string LogicalIndexNumberName = "LogicalIndexNumber";

		public const string PropertyBlobName = "PropertyBlob";

		public const string LargePropertyValueBlobName = "LargePropertyValueBlob";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PropertyBagName = "PropertyBag";

		public const string AclTableAndSecurityDescriptorName = "AclTableAndSecurityDescriptor";

		public const string MidsetDeletedDeltaName = "MidsetDeletedDelta";

		public const string ReservedDocumentIdCurrentName = "ReservedDocumentIdCurrent";

		public const string ReservedDocumentIdMaxName = "ReservedDocumentIdMax";

		public const string MailboxNumberName = "MailboxNumber";

		public const string PhysicalTableName = "Folder";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn folderId;

		private PhysicalColumn lcnCurrent;

		private PhysicalColumn versionHistory;

		private PhysicalColumn parentFolderId;

		private PhysicalColumn creationTime;

		private PhysicalColumn creatorSid;

		private PhysicalColumn lastModificationTime;

		private PhysicalColumn sourceKey;

		private PhysicalColumn changeKey;

		private PhysicalColumn localCommitTimeMax;

		private PhysicalColumn lastModifierSid;

		private PhysicalColumn displayName;

		private PhysicalColumn comment;

		private PhysicalColumn containerClass;

		private PhysicalColumn messageCount;

		private PhysicalColumn totalDeletedCount;

		private PhysicalColumn unreadMessageCount;

		private PhysicalColumn virtualUnreadMessageCount;

		private PhysicalColumn messageSize;

		private PhysicalColumn messageAttachCount;

		private PhysicalColumn messageHasAttachCount;

		private PhysicalColumn hiddenItemCount;

		private PhysicalColumn unreadHiddenItemCount;

		private PhysicalColumn hiddenItemSize;

		private PhysicalColumn hiddenItemHasAttachCount;

		private PhysicalColumn hiddenItemAttachCount;

		private PhysicalColumn displayType;

		private PhysicalColumn conversationCount;

		private PhysicalColumn folderCount;

		private PhysicalColumn specialFolderNumber;

		private PhysicalColumn reservedMessageIdGlobCntCurrent;

		private PhysicalColumn reservedMessageIdGlobCntMax;

		private PhysicalColumn reservedMessageCnGlobCntCurrent;

		private PhysicalColumn reservedMessageCnGlobCntMax;

		private PhysicalColumn nextArticleNumber;

		private PhysicalColumn midsetDeleted;

		private PhysicalColumn normalItemPromotedColumns;

		private PhysicalColumn hiddenItemPromotedColumns;

		private PhysicalColumn promotionTimestamp;

		private PhysicalColumn promotionUseHistory;

		private PhysicalColumn queryCriteria;

		private PhysicalColumn searchState;

		private PhysicalColumn setSearchCriteriaFlags;

		private PhysicalColumn scopeFolders;

		private PhysicalColumn nonRecursiveSearchBacklinks;

		private PhysicalColumn recursiveSearchBacklinks;

		private PhysicalColumn logicalIndexNumber;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn largePropertyValueBlob;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn propertyBag;

		private PhysicalColumn aclTableAndSecurityDescriptor;

		private PhysicalColumn midsetDeletedDelta;

		private PhysicalColumn reservedDocumentIdCurrent;

		private PhysicalColumn reservedDocumentIdMax;

		private PhysicalColumn mailboxNumber;

		private Index folderPK;

		private Index folderChangeNumberIndex;

		private Index folderByParentIndex;

		private Table table;
	}
}
