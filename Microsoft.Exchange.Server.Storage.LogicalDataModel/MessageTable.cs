using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class MessageTable
	{
		internal MessageTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.messageDocumentId = Factory.CreatePhysicalColumn("MessageDocumentId", "MessageDocumentId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.messageId = Factory.CreatePhysicalColumn("MessageId", "MessageId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.lcnCurrent = Factory.CreatePhysicalColumn("LcnCurrent", "LcnCurrent", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.versionHistory = Factory.CreatePhysicalColumn("VersionHistory", "VersionHistory", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1048576, 0, 1048576);
			this.groupCns = Factory.CreatePhysicalColumn("GroupCns", "GroupCns", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.lastModificationTime = Factory.CreatePhysicalColumn("LastModificationTime", "LastModificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lcnReadUnread = Factory.CreatePhysicalColumn("LcnReadUnread", "LcnReadUnread", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.sourceKey = Factory.CreatePhysicalColumn("SourceKey", "SourceKey", typeof(byte[]), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.changeKey = Factory.CreatePhysicalColumn("ChangeKey", "ChangeKey", typeof(byte[]), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.size = Factory.CreatePhysicalColumn("Size", "Size", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.recipientList = Factory.CreatePhysicalColumn("RecipientList", "RecipientList", typeof(byte[][]), true, false, false, true, false, Visibility.Redacted, 1073741824, 0, 50);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 3110);
			this.offPagePropertyBlob = Factory.CreatePhysicalColumn("OffPagePropertyBlob", "OffPagePropertyBlob", typeof(byte[]), true, false, true, true, false, Visibility.Redacted, 1073741824, 0, 100);
			this.largePropertyValueBlob = Factory.CreatePhysicalColumn("LargePropertyValueBlob", "LargePropertyValueBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.subobjectsBlob = Factory.CreatePhysicalColumn("SubobjectsBlob", "SubobjectsBlob", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 50);
			this.isHidden = Factory.CreatePhysicalColumn("IsHidden", "IsHidden", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.isRead = Factory.CreatePhysicalColumn("IsRead", "IsRead", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.virtualIsRead = Factory.CreatePhysicalColumn("VirtualIsRead", "VirtualIsRead", typeof(bool), true, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.virtualUnreadMessageCount = Factory.CreatePhysicalColumn("VirtualUnreadMessageCount", "VirtualUnreadMessageCount", typeof(long), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hasAttachments = Factory.CreatePhysicalColumn("HasAttachments", "HasAttachments", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.conversationIndexTracking = Factory.CreatePhysicalColumn("ConversationIndexTracking", "ConversationIndexTracking", typeof(bool), true, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.messageFlagsActual = Factory.CreatePhysicalColumn("MessageFlagsActual", "MessageFlagsActual", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailFlags = Factory.CreatePhysicalColumn("MailFlags", "MailFlags", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.status = Factory.CreatePhysicalColumn("Status", "Status", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.messageClass = Factory.CreatePhysicalColumn("MessageClass", "MessageClass", typeof(string), true, false, false, false, false, Visibility.Public, 255, 0, 255);
			this.importance = Factory.CreatePhysicalColumn("Importance", "Importance", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.sensitivity = Factory.CreatePhysicalColumn("Sensitivity", "Sensitivity", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.priority = Factory.CreatePhysicalColumn("Priority", "Priority", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.conversationIndex = Factory.CreatePhysicalColumn("ConversationIndex", "ConversationIndex", typeof(byte[]), true, false, false, false, false, Visibility.Public, 4096, 0, 4096);
			this.conversationMembers = Factory.CreatePhysicalColumn("ConversationMembers", "ConversationMembers", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.subjectPrefix = Factory.CreatePhysicalColumn("SubjectPrefix", "SubjectPrefix", typeof(string), true, false, false, false, false, Visibility.Public, 1024, 0, 1024);
			this.articleNumber = Factory.CreatePhysicalColumn("ArticleNumber", "ArticleNumber", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.iMAPId = Factory.CreatePhysicalColumn("IMAPId", "IMAPId", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.displayTo = Factory.CreatePhysicalColumn("DisplayTo", "DisplayTo", typeof(string), true, false, false, false, false, Visibility.Public, 30720, 0, 256);
			this.conversationDocumentId = Factory.CreatePhysicalColumn("ConversationDocumentId", "ConversationDocumentId", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.bodyType = Factory.CreatePhysicalColumn("BodyType", "BodyType", typeof(short), true, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.dateCreated = Factory.CreatePhysicalColumn("DateCreated", "DateCreated", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.codePage = Factory.CreatePhysicalColumn("CodePage", "CodePage", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.dateReceived = Factory.CreatePhysicalColumn("DateReceived", "DateReceived", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.dateSent = Factory.CreatePhysicalColumn("DateSent", "DateSent", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.nativeBody = Factory.CreatePhysicalColumn("NativeBody", "NativeBody", typeof(byte[]), true, false, true, true, false, Visibility.Private, 1073741824, 0, 50);
			this.annotationToken = Factory.CreatePhysicalColumn("AnnotationToken", "AnnotationToken", typeof(byte[]), true, false, true, true, false, Visibility.Private, 1073741824, 0, 20);
			this.userConfigurationXmlStream = Factory.CreatePhysicalColumn("UserConfigurationXmlStream", "UserConfigurationXmlStream", typeof(byte[]), true, false, true, true, false, Visibility.Private, 1073741824, 0, 20);
			this.userConfigurationStream = Factory.CreatePhysicalColumn("UserConfigurationStream", "UserConfigurationStream", typeof(byte[]), true, false, true, true, false, Visibility.Private, 1073741824, 0, 20);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.propertyBag = Factory.CreatePhysicalColumn("PropertyBag", "PropertyBag", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 4, 0, 4);
			this.conversationId = Factory.CreatePhysicalColumn("ConversationId", "ConversationId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.virtualParentDisplay = Factory.CreatePhysicalColumn("VirtualParentDisplay", "VirtualParentDisplay", typeof(string), true, false, false, false, false, Visibility.Public, 512, 0, 512);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string name = "MessagePK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.messagePK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.MessageDocumentId
			});
			string name2 = "MessageUnique";
			bool primaryKey2 = false;
			bool unique2 = true;
			bool schemaExtension2 = false;
			bool[] conditional2 = new bool[4];
			this.messageUnique = new Index(name2, primaryKey2, unique2, schemaExtension2, conditional2, new bool[]
			{
				true,
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.IsHidden,
				this.MessageId
			});
			string name3 = "MessageIdUnique";
			bool primaryKey3 = false;
			bool unique3 = true;
			bool schemaExtension3 = false;
			bool[] conditional3 = new bool[2];
			this.messageIdUnique = new Index(name3, primaryKey3, unique3, schemaExtension3, conditional3, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.MessageId
			});
			this.conversationIdUnique = new Index("ConversationIdUnique", false, true, false, new bool[]
			{
				default(bool),
				true
			}, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.ConversationId
			});
			this.iMAPIDUnique = new Index("IMAPIDUnique", false, true, false, new bool[]
			{
				default(bool),
				true,
				true
			}, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.IMAPId
			});
			this.articleNumberUnique = new Index("ArticleNumberUnique", false, true, false, new bool[]
			{
				default(bool),
				true,
				true
			}, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.ArticleNumber
			});
			Index[] indexes = new Index[]
			{
				this.MessagePK,
				this.MessageUnique,
				this.ConversationIdUnique
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, this.OffPagePropertyBlob, this.PropertyBag, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[]
			{
				this.VirtualIsRead,
				this.VirtualUnreadMessageCount,
				this.PropertyBag,
				this.VirtualParentDisplay
			};
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.MessageDocumentId,
				this.MessageId,
				this.FolderId,
				this.LcnCurrent,
				this.VersionHistory,
				this.GroupCns,
				this.LastModificationTime,
				this.LcnReadUnread,
				this.SourceKey,
				this.ChangeKey,
				this.Size,
				this.RecipientList,
				this.PropertyBlob,
				this.OffPagePropertyBlob,
				this.LargePropertyValueBlob,
				this.SubobjectsBlob,
				this.IsHidden,
				this.IsRead,
				this.HasAttachments,
				this.ConversationIndexTracking,
				this.MessageFlagsActual,
				this.MailFlags,
				this.Status,
				this.MessageClass,
				this.Importance,
				this.Sensitivity,
				this.Priority,
				this.ConversationIndex,
				this.ConversationMembers,
				this.SubjectPrefix,
				this.ArticleNumber,
				this.IMAPId,
				this.DisplayTo,
				this.ConversationDocumentId,
				this.BodyType,
				this.DateCreated,
				this.CodePage,
				this.DateReceived,
				this.DateSent,
				this.NativeBody,
				this.AnnotationToken,
				this.UserConfigurationXmlStream,
				this.UserConfigurationStream,
				this.ExtensionBlob,
				this.ConversationId,
				this.MailboxNumber
			};
			this.table = Factory.CreateTable("Message", TableClass.Message, CultureHelper.DefaultCultureInfo, false, TableAccessHints.None, false, Visibility.Redacted, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn MessageDocumentId
		{
			get
			{
				return this.messageDocumentId;
			}
		}

		public PhysicalColumn MessageId
		{
			get
			{
				return this.messageId;
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

		public PhysicalColumn GroupCns
		{
			get
			{
				return this.groupCns;
			}
		}

		public PhysicalColumn LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public PhysicalColumn LcnReadUnread
		{
			get
			{
				return this.lcnReadUnread;
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

		public PhysicalColumn Size
		{
			get
			{
				return this.size;
			}
		}

		public PhysicalColumn RecipientList
		{
			get
			{
				return this.recipientList;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn OffPagePropertyBlob
		{
			get
			{
				return this.offPagePropertyBlob;
			}
		}

		public PhysicalColumn LargePropertyValueBlob
		{
			get
			{
				return this.largePropertyValueBlob;
			}
		}

		public PhysicalColumn SubobjectsBlob
		{
			get
			{
				return this.subobjectsBlob;
			}
		}

		public PhysicalColumn IsHidden
		{
			get
			{
				return this.isHidden;
			}
		}

		public PhysicalColumn IsRead
		{
			get
			{
				return this.isRead;
			}
		}

		public PhysicalColumn VirtualIsRead
		{
			get
			{
				return this.virtualIsRead;
			}
		}

		public PhysicalColumn VirtualUnreadMessageCount
		{
			get
			{
				return this.virtualUnreadMessageCount;
			}
		}

		public PhysicalColumn HasAttachments
		{
			get
			{
				return this.hasAttachments;
			}
		}

		public PhysicalColumn ConversationIndexTracking
		{
			get
			{
				return this.conversationIndexTracking;
			}
		}

		public PhysicalColumn MessageFlagsActual
		{
			get
			{
				return this.messageFlagsActual;
			}
		}

		public PhysicalColumn MailFlags
		{
			get
			{
				return this.mailFlags;
			}
		}

		public PhysicalColumn Status
		{
			get
			{
				return this.status;
			}
		}

		public PhysicalColumn MessageClass
		{
			get
			{
				return this.messageClass;
			}
		}

		public PhysicalColumn Importance
		{
			get
			{
				return this.importance;
			}
		}

		public PhysicalColumn Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
		}

		public PhysicalColumn Priority
		{
			get
			{
				return this.priority;
			}
		}

		public PhysicalColumn ConversationIndex
		{
			get
			{
				return this.conversationIndex;
			}
		}

		public PhysicalColumn ConversationMembers
		{
			get
			{
				return this.conversationMembers;
			}
		}

		public PhysicalColumn SubjectPrefix
		{
			get
			{
				return this.subjectPrefix;
			}
		}

		public PhysicalColumn ArticleNumber
		{
			get
			{
				return this.articleNumber;
			}
		}

		public PhysicalColumn IMAPId
		{
			get
			{
				return this.iMAPId;
			}
		}

		public PhysicalColumn DisplayTo
		{
			get
			{
				return this.displayTo;
			}
		}

		public PhysicalColumn ConversationDocumentId
		{
			get
			{
				return this.conversationDocumentId;
			}
		}

		public PhysicalColumn BodyType
		{
			get
			{
				return this.bodyType;
			}
		}

		public PhysicalColumn DateCreated
		{
			get
			{
				return this.dateCreated;
			}
		}

		public PhysicalColumn CodePage
		{
			get
			{
				return this.codePage;
			}
		}

		public PhysicalColumn DateReceived
		{
			get
			{
				return this.dateReceived;
			}
		}

		public PhysicalColumn DateSent
		{
			get
			{
				return this.dateSent;
			}
		}

		public PhysicalColumn NativeBody
		{
			get
			{
				return this.nativeBody;
			}
		}

		public PhysicalColumn AnnotationToken
		{
			get
			{
				return this.annotationToken;
			}
		}

		public PhysicalColumn UserConfigurationXmlStream
		{
			get
			{
				return this.userConfigurationXmlStream;
			}
		}

		public PhysicalColumn UserConfigurationStream
		{
			get
			{
				return this.userConfigurationStream;
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

		public PhysicalColumn ConversationId
		{
			get
			{
				return this.conversationId;
			}
		}

		public PhysicalColumn VirtualParentDisplay
		{
			get
			{
				return this.virtualParentDisplay;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Index MessagePK
		{
			get
			{
				return this.messagePK;
			}
		}

		public Index MessageUnique
		{
			get
			{
				return this.messageUnique;
			}
		}

		public Index MessageIdUnique
		{
			get
			{
				return this.messageIdUnique;
			}
		}

		public Index ConversationIdUnique
		{
			get
			{
				return this.conversationIdUnique;
			}
		}

		public Index IMAPIDUnique
		{
			get
			{
				return this.iMAPIDUnique;
			}
		}

		public Index ArticleNumberUnique
		{
			get
			{
				return this.articleNumberUnique;
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
			physicalColumn = this.messageDocumentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageDocumentId = null;
			}
			physicalColumn = this.messageId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageId = null;
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
			physicalColumn = this.groupCns;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.groupCns = null;
			}
			physicalColumn = this.lastModificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModificationTime = null;
			}
			physicalColumn = this.lcnReadUnread;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lcnReadUnread = null;
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
			physicalColumn = this.size;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.size = null;
			}
			physicalColumn = this.recipientList;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.recipientList = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
			}
			physicalColumn = this.offPagePropertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.offPagePropertyBlob = null;
			}
			physicalColumn = this.largePropertyValueBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.largePropertyValueBlob = null;
			}
			physicalColumn = this.subobjectsBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.subobjectsBlob = null;
			}
			physicalColumn = this.isHidden;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.isHidden = null;
			}
			physicalColumn = this.isRead;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.isRead = null;
			}
			physicalColumn = this.virtualIsRead;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.virtualIsRead = null;
			}
			physicalColumn = this.virtualUnreadMessageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.virtualUnreadMessageCount = null;
			}
			physicalColumn = this.hasAttachments;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hasAttachments = null;
			}
			physicalColumn = this.conversationIndexTracking;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationIndexTracking = null;
			}
			physicalColumn = this.messageFlagsActual;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageFlagsActual = null;
			}
			physicalColumn = this.mailFlags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailFlags = null;
			}
			physicalColumn = this.status;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.status = null;
			}
			physicalColumn = this.messageClass;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageClass = null;
			}
			physicalColumn = this.importance;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.importance = null;
			}
			physicalColumn = this.sensitivity;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.sensitivity = null;
			}
			physicalColumn = this.priority;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.priority = null;
			}
			physicalColumn = this.conversationIndex;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationIndex = null;
			}
			physicalColumn = this.conversationMembers;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationMembers = null;
			}
			physicalColumn = this.subjectPrefix;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.subjectPrefix = null;
			}
			physicalColumn = this.articleNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.articleNumber = null;
			}
			physicalColumn = this.iMAPId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.iMAPId = null;
			}
			physicalColumn = this.displayTo;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.displayTo = null;
			}
			physicalColumn = this.conversationDocumentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationDocumentId = null;
			}
			physicalColumn = this.bodyType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.bodyType = null;
			}
			physicalColumn = this.dateCreated;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.dateCreated = null;
			}
			physicalColumn = this.codePage;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.codePage = null;
			}
			physicalColumn = this.dateReceived;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.dateReceived = null;
			}
			physicalColumn = this.dateSent;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.dateSent = null;
			}
			physicalColumn = this.nativeBody;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.nativeBody = null;
			}
			physicalColumn = this.annotationToken;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.annotationToken = null;
			}
			physicalColumn = this.userConfigurationXmlStream;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.userConfigurationXmlStream = null;
			}
			physicalColumn = this.userConfigurationStream;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.userConfigurationStream = null;
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
			physicalColumn = this.conversationId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationId = null;
			}
			physicalColumn = this.virtualParentDisplay;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.virtualParentDisplay = null;
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
			Index index = this.messagePK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.messagePK = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.messageUnique;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.messageUnique = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.messageIdUnique;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.messageIdUnique = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.conversationIdUnique;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.conversationIdUnique = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.iMAPIDUnique;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.iMAPIDUnique = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.articleNumberUnique;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.articleNumberUnique = null;
				this.Table.Indexes.Remove(index);
			}
		}

		internal void SetupFullTextIndex(StoreDatabase database)
		{
			this.messageFullText = new FullTextIndex("MessageFullText", new Column[]
			{
				this.MailboxPartitionNumber,
				this.MessageDocumentId
			});
			this.table.FullTextIndex = this.messageFullText;
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string MessageDocumentIdName = "MessageDocumentId";

		public const string MessageIdName = "MessageId";

		public const string FolderIdName = "FolderId";

		public const string LcnCurrentName = "LcnCurrent";

		public const string VersionHistoryName = "VersionHistory";

		public const string GroupCnsName = "GroupCns";

		public const string LastModificationTimeName = "LastModificationTime";

		public const string LcnReadUnreadName = "LcnReadUnread";

		public const string SourceKeyName = "SourceKey";

		public const string ChangeKeyName = "ChangeKey";

		public const string SizeName = "Size";

		public const string RecipientListName = "RecipientList";

		public const string PropertyBlobName = "PropertyBlob";

		public const string OffPagePropertyBlobName = "OffPagePropertyBlob";

		public const string LargePropertyValueBlobName = "LargePropertyValueBlob";

		public const string SubobjectsBlobName = "SubobjectsBlob";

		public const string IsHiddenName = "IsHidden";

		public const string IsReadName = "IsRead";

		public const string VirtualIsReadName = "VirtualIsRead";

		public const string VirtualUnreadMessageCountName = "VirtualUnreadMessageCount";

		public const string HasAttachmentsName = "HasAttachments";

		public const string ConversationIndexTrackingName = "ConversationIndexTracking";

		public const string MessageFlagsActualName = "MessageFlagsActual";

		public const string MailFlagsName = "MailFlags";

		public const string StatusName = "Status";

		public const string MessageClassName = "MessageClass";

		public const string ImportanceName = "Importance";

		public const string SensitivityName = "Sensitivity";

		public const string PriorityName = "Priority";

		public const string ConversationIndexName = "ConversationIndex";

		public const string ConversationMembersName = "ConversationMembers";

		public const string SubjectPrefixName = "SubjectPrefix";

		public const string ArticleNumberName = "ArticleNumber";

		public const string IMAPIdName = "IMAPId";

		public const string DisplayToName = "DisplayTo";

		public const string ConversationDocumentIdName = "ConversationDocumentId";

		public const string BodyTypeName = "BodyType";

		public const string DateCreatedName = "DateCreated";

		public const string CodePageName = "CodePage";

		public const string DateReceivedName = "DateReceived";

		public const string DateSentName = "DateSent";

		public const string NativeBodyName = "NativeBody";

		public const string AnnotationTokenName = "AnnotationToken";

		public const string UserConfigurationXmlStreamName = "UserConfigurationXmlStream";

		public const string UserConfigurationStreamName = "UserConfigurationStream";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PropertyBagName = "PropertyBag";

		public const string ConversationIdName = "ConversationId";

		public const string VirtualParentDisplayName = "VirtualParentDisplay";

		public const string MailboxNumberName = "MailboxNumber";

		public const string PhysicalTableName = "Message";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn messageDocumentId;

		private PhysicalColumn messageId;

		private PhysicalColumn folderId;

		private PhysicalColumn lcnCurrent;

		private PhysicalColumn versionHistory;

		private PhysicalColumn groupCns;

		private PhysicalColumn lastModificationTime;

		private PhysicalColumn lcnReadUnread;

		private PhysicalColumn sourceKey;

		private PhysicalColumn changeKey;

		private PhysicalColumn size;

		private PhysicalColumn recipientList;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn offPagePropertyBlob;

		private PhysicalColumn largePropertyValueBlob;

		private PhysicalColumn subobjectsBlob;

		private PhysicalColumn isHidden;

		private PhysicalColumn isRead;

		private PhysicalColumn virtualIsRead;

		private PhysicalColumn virtualUnreadMessageCount;

		private PhysicalColumn hasAttachments;

		private PhysicalColumn conversationIndexTracking;

		private PhysicalColumn messageFlagsActual;

		private PhysicalColumn mailFlags;

		private PhysicalColumn status;

		private PhysicalColumn messageClass;

		private PhysicalColumn importance;

		private PhysicalColumn sensitivity;

		private PhysicalColumn priority;

		private PhysicalColumn conversationIndex;

		private PhysicalColumn conversationMembers;

		private PhysicalColumn subjectPrefix;

		private PhysicalColumn articleNumber;

		private PhysicalColumn iMAPId;

		private PhysicalColumn displayTo;

		private PhysicalColumn conversationDocumentId;

		private PhysicalColumn bodyType;

		private PhysicalColumn dateCreated;

		private PhysicalColumn codePage;

		private PhysicalColumn dateReceived;

		private PhysicalColumn dateSent;

		private PhysicalColumn nativeBody;

		private PhysicalColumn annotationToken;

		private PhysicalColumn userConfigurationXmlStream;

		private PhysicalColumn userConfigurationStream;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn propertyBag;

		private PhysicalColumn conversationId;

		private PhysicalColumn virtualParentDisplay;

		private PhysicalColumn mailboxNumber;

		private Index messagePK;

		private Index messageUnique;

		private Index messageIdUnique;

		private Index conversationIdUnique;

		private Index iMAPIDUnique;

		private Index articleNumberUnique;

		private FullTextIndex messageFullText;

		private Table table;
	}
}
