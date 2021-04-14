using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class MailboxTable
	{
		internal MailboxTable()
		{
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailboxGuid = Factory.CreatePhysicalColumn("MailboxGuid", "MailboxGuid", typeof(Guid), true, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.ownerADGuid = Factory.CreatePhysicalColumn("OwnerADGuid", "OwnerADGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.ownerLegacyDN = Factory.CreatePhysicalColumn("OwnerLegacyDN", "OwnerLegacyDN", typeof(string), false, false, false, false, false, Visibility.Redacted, 1024, 0, 1024);
			this.mailboxInstanceGuid = Factory.CreatePhysicalColumn("MailboxInstanceGuid", "MailboxInstanceGuid", typeof(Guid), true, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.mappingSignatureGuid = Factory.CreatePhysicalColumn("MappingSignatureGuid", "MappingSignatureGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.mailboxOwnerDisplayName = Factory.CreatePhysicalColumn("MailboxOwnerDisplayName", "MailboxOwnerDisplayName", typeof(string), true, false, false, false, false, Visibility.Redacted, 1024, 0, 1024);
			this.displayName = Factory.CreatePhysicalColumn("DisplayName", "DisplayName", typeof(string), true, false, false, false, false, Visibility.Redacted, 1024, 0, 1024);
			this.simpleDisplayName = Factory.CreatePhysicalColumn("SimpleDisplayName", "SimpleDisplayName", typeof(string), true, false, false, false, false, Visibility.Redacted, 128, 0, 128);
			this.comment = Factory.CreatePhysicalColumn("Comment", "Comment", typeof(string), true, false, false, false, false, Visibility.Redacted, 1024, 0, 1024);
			this.oofState = Factory.CreatePhysicalColumn("OofState", "OofState", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.deletedOn = Factory.CreatePhysicalColumn("DeletedOn", "DeletedOn", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.status = Factory.CreatePhysicalColumn("Status", "Status", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.lcid = Factory.CreatePhysicalColumn("Lcid", "Lcid", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.propertyBag = Factory.CreatePhysicalColumn("PropertyBag", "PropertyBag", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 4, 0, 4);
			this.largePropertyValueBlob = Factory.CreatePhysicalColumn("LargePropertyValueBlob", "LargePropertyValueBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.messageCount = Factory.CreatePhysicalColumn("MessageCount", "MessageCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageSize = Factory.CreatePhysicalColumn("MessageSize", "MessageSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenMessageCount = Factory.CreatePhysicalColumn("HiddenMessageCount", "HiddenMessageCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenMessageSize = Factory.CreatePhysicalColumn("HiddenMessageSize", "HiddenMessageSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageDeletedCount = Factory.CreatePhysicalColumn("MessageDeletedCount", "MessageDeletedCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageDeletedSize = Factory.CreatePhysicalColumn("MessageDeletedSize", "MessageDeletedSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenMessageDeletedCount = Factory.CreatePhysicalColumn("HiddenMessageDeletedCount", "HiddenMessageDeletedCount", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.hiddenMessageDeletedSize = Factory.CreatePhysicalColumn("HiddenMessageDeletedSize", "HiddenMessageDeletedSize", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastLogonTime = Factory.CreatePhysicalColumn("LastLogonTime", "LastLogonTime", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastLogoffTime = Factory.CreatePhysicalColumn("LastLogoffTime", "LastLogoffTime", typeof(DateTime), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.conversationEnabled = Factory.CreatePhysicalColumn("ConversationEnabled", "ConversationEnabled", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.mailboxDatabaseVersion = Factory.CreatePhysicalColumn("MailboxDatabaseVersion", "MailboxDatabaseVersion", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.lastQuotaNotificationTime = Factory.CreatePhysicalColumn("LastQuotaNotificationTime", "LastQuotaNotificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.preservingMailboxSignature = Factory.CreatePhysicalColumn("PreservingMailboxSignature", "PreservingMailboxSignature", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.tenantHint = Factory.CreatePhysicalColumn("TenantHint", "TenantHint", typeof(byte[]), true, false, false, false, false, Visibility.Public, 128, 0, 128);
			this.nextMessageDocumentId = Factory.CreatePhysicalColumn("NextMessageDocumentId", "NextMessageDocumentId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.defaultPromotedMessagePropertyIds = Factory.CreatePhysicalColumn("DefaultPromotedMessagePropertyIds", "DefaultPromotedMessagePropertyIds", typeof(short[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.alwaysPromotedMessagePropertyIds = Factory.CreatePhysicalColumn("AlwaysPromotedMessagePropertyIds", "AlwaysPromotedMessagePropertyIds", typeof(short[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.lastMailboxMaintenanceTime = Factory.CreatePhysicalColumn("LastMailboxMaintenanceTime", "LastMailboxMaintenanceTime", typeof(DateTime), true, false, false, false, true, Visibility.Public, 0, 8, 8);
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			this.unifiedMailboxGuid = Factory.CreatePhysicalColumn("UnifiedMailboxGuid", "UnifiedMailboxGuid", typeof(Guid), true, false, false, false, true, Visibility.Public, 0, 16, 16);
			string name = "MailboxTablePK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.mailboxTablePK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.MailboxNumber
			});
			this.mailboxGuidIndex = new Index("MailboxGuidIndex", false, true, false, new bool[]
			{
				true
			}, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.MailboxGuid
			});
			this.unifiedMailboxGuidIndex = new Index("UnifiedMailboxGuidIndex", false, false, true, new bool[]
			{
				true
			}, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.UnifiedMailboxGuid
			});
			Index[] indexes = new Index[]
			{
				this.MailboxTablePK,
				this.MailboxGuidIndex
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, null, this.PropertyBag, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[]
			{
				this.PropertyBag
			};
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.MailboxGuid,
				this.OwnerADGuid,
				this.OwnerLegacyDN,
				this.MailboxInstanceGuid,
				this.MappingSignatureGuid,
				this.MailboxOwnerDisplayName,
				this.DisplayName,
				this.SimpleDisplayName,
				this.Comment,
				this.OofState,
				this.DeletedOn,
				this.Status,
				this.Lcid,
				this.PropertyBlob,
				this.LargePropertyValueBlob,
				this.MessageCount,
				this.MessageSize,
				this.HiddenMessageCount,
				this.HiddenMessageSize,
				this.MessageDeletedCount,
				this.MessageDeletedSize,
				this.HiddenMessageDeletedCount,
				this.HiddenMessageDeletedSize,
				this.LastLogonTime,
				this.LastLogoffTime,
				this.ConversationEnabled,
				this.MailboxDatabaseVersion,
				this.LastQuotaNotificationTime,
				this.PreservingMailboxSignature,
				this.TenantHint,
				this.NextMessageDocumentId,
				this.DefaultPromotedMessagePropertyIds,
				this.AlwaysPromotedMessagePropertyIds,
				this.ExtensionBlob,
				this.LastMailboxMaintenanceTime,
				this.MailboxPartitionNumber,
				this.UnifiedMailboxGuid
			};
			this.table = Factory.CreateTable("Mailbox", TableClass.Mailbox, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public PhysicalColumn OwnerADGuid
		{
			get
			{
				return this.ownerADGuid;
			}
		}

		public PhysicalColumn OwnerLegacyDN
		{
			get
			{
				return this.ownerLegacyDN;
			}
		}

		public PhysicalColumn MailboxInstanceGuid
		{
			get
			{
				return this.mailboxInstanceGuid;
			}
		}

		public PhysicalColumn MappingSignatureGuid
		{
			get
			{
				return this.mappingSignatureGuid;
			}
		}

		public PhysicalColumn MailboxOwnerDisplayName
		{
			get
			{
				return this.mailboxOwnerDisplayName;
			}
		}

		public PhysicalColumn DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public PhysicalColumn SimpleDisplayName
		{
			get
			{
				return this.simpleDisplayName;
			}
		}

		public PhysicalColumn Comment
		{
			get
			{
				return this.comment;
			}
		}

		public PhysicalColumn OofState
		{
			get
			{
				return this.oofState;
			}
		}

		public PhysicalColumn DeletedOn
		{
			get
			{
				return this.deletedOn;
			}
		}

		public PhysicalColumn Status
		{
			get
			{
				return this.status;
			}
		}

		public PhysicalColumn Lcid
		{
			get
			{
				return this.lcid;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn PropertyBag
		{
			get
			{
				return this.propertyBag;
			}
		}

		public PhysicalColumn LargePropertyValueBlob
		{
			get
			{
				return this.largePropertyValueBlob;
			}
		}

		public PhysicalColumn MessageCount
		{
			get
			{
				return this.messageCount;
			}
		}

		public PhysicalColumn MessageSize
		{
			get
			{
				return this.messageSize;
			}
		}

		public PhysicalColumn HiddenMessageCount
		{
			get
			{
				return this.hiddenMessageCount;
			}
		}

		public PhysicalColumn HiddenMessageSize
		{
			get
			{
				return this.hiddenMessageSize;
			}
		}

		public PhysicalColumn MessageDeletedCount
		{
			get
			{
				return this.messageDeletedCount;
			}
		}

		public PhysicalColumn MessageDeletedSize
		{
			get
			{
				return this.messageDeletedSize;
			}
		}

		public PhysicalColumn HiddenMessageDeletedCount
		{
			get
			{
				return this.hiddenMessageDeletedCount;
			}
		}

		public PhysicalColumn HiddenMessageDeletedSize
		{
			get
			{
				return this.hiddenMessageDeletedSize;
			}
		}

		public PhysicalColumn LastLogonTime
		{
			get
			{
				return this.lastLogonTime;
			}
		}

		public PhysicalColumn LastLogoffTime
		{
			get
			{
				return this.lastLogoffTime;
			}
		}

		public PhysicalColumn ConversationEnabled
		{
			get
			{
				return this.conversationEnabled;
			}
		}

		public PhysicalColumn MailboxDatabaseVersion
		{
			get
			{
				return this.mailboxDatabaseVersion;
			}
		}

		public PhysicalColumn LastQuotaNotificationTime
		{
			get
			{
				return this.lastQuotaNotificationTime;
			}
		}

		public PhysicalColumn PreservingMailboxSignature
		{
			get
			{
				return this.preservingMailboxSignature;
			}
		}

		public PhysicalColumn TenantHint
		{
			get
			{
				return this.tenantHint;
			}
		}

		public PhysicalColumn NextMessageDocumentId
		{
			get
			{
				return this.nextMessageDocumentId;
			}
		}

		public PhysicalColumn DefaultPromotedMessagePropertyIds
		{
			get
			{
				return this.defaultPromotedMessagePropertyIds;
			}
		}

		public PhysicalColumn AlwaysPromotedMessagePropertyIds
		{
			get
			{
				return this.alwaysPromotedMessagePropertyIds;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn LastMailboxMaintenanceTime
		{
			get
			{
				return this.lastMailboxMaintenanceTime;
			}
		}

		public PhysicalColumn MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public PhysicalColumn UnifiedMailboxGuid
		{
			get
			{
				return this.unifiedMailboxGuid;
			}
		}

		public Index MailboxTablePK
		{
			get
			{
				return this.mailboxTablePK;
			}
		}

		public Index MailboxGuidIndex
		{
			get
			{
				return this.mailboxGuidIndex;
			}
		}

		public Index UnifiedMailboxGuidIndex
		{
			get
			{
				return this.unifiedMailboxGuidIndex;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			physicalColumn = this.mailboxGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxGuid = null;
			}
			physicalColumn = this.ownerADGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.ownerADGuid = null;
			}
			physicalColumn = this.ownerLegacyDN;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.ownerLegacyDN = null;
			}
			physicalColumn = this.mailboxInstanceGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxInstanceGuid = null;
			}
			physicalColumn = this.mappingSignatureGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mappingSignatureGuid = null;
			}
			physicalColumn = this.mailboxOwnerDisplayName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxOwnerDisplayName = null;
			}
			physicalColumn = this.displayName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.displayName = null;
			}
			physicalColumn = this.simpleDisplayName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.simpleDisplayName = null;
			}
			physicalColumn = this.comment;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.comment = null;
			}
			physicalColumn = this.oofState;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.oofState = null;
			}
			physicalColumn = this.deletedOn;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.deletedOn = null;
			}
			physicalColumn = this.status;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.status = null;
			}
			physicalColumn = this.lcid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lcid = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
			}
			physicalColumn = this.propertyBag;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBag = null;
			}
			physicalColumn = this.largePropertyValueBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.largePropertyValueBlob = null;
			}
			physicalColumn = this.messageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageCount = null;
			}
			physicalColumn = this.messageSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageSize = null;
			}
			physicalColumn = this.hiddenMessageCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenMessageCount = null;
			}
			physicalColumn = this.hiddenMessageSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenMessageSize = null;
			}
			physicalColumn = this.messageDeletedCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageDeletedCount = null;
			}
			physicalColumn = this.messageDeletedSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageDeletedSize = null;
			}
			physicalColumn = this.hiddenMessageDeletedCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenMessageDeletedCount = null;
			}
			physicalColumn = this.hiddenMessageDeletedSize;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.hiddenMessageDeletedSize = null;
			}
			physicalColumn = this.lastLogonTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastLogonTime = null;
			}
			physicalColumn = this.lastLogoffTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastLogoffTime = null;
			}
			physicalColumn = this.conversationEnabled;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conversationEnabled = null;
			}
			physicalColumn = this.mailboxDatabaseVersion;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxDatabaseVersion = null;
			}
			physicalColumn = this.lastQuotaNotificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastQuotaNotificationTime = null;
			}
			physicalColumn = this.preservingMailboxSignature;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.preservingMailboxSignature = null;
			}
			physicalColumn = this.tenantHint;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.tenantHint = null;
			}
			physicalColumn = this.nextMessageDocumentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.nextMessageDocumentId = null;
			}
			physicalColumn = this.defaultPromotedMessagePropertyIds;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.defaultPromotedMessagePropertyIds = null;
			}
			physicalColumn = this.alwaysPromotedMessagePropertyIds;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.alwaysPromotedMessagePropertyIds = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.lastMailboxMaintenanceTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastMailboxMaintenanceTime = null;
			}
			physicalColumn = this.mailboxPartitionNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxPartitionNumber = null;
			}
			physicalColumn = this.unifiedMailboxGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.unifiedMailboxGuid = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.mailboxTablePK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.mailboxTablePK = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.mailboxGuidIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.mailboxGuidIndex = null;
				this.Table.Indexes.Remove(index);
			}
			index = this.unifiedMailboxGuidIndex;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.unifiedMailboxGuidIndex = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxNumberName = "MailboxNumber";

		public const string MailboxGuidName = "MailboxGuid";

		public const string OwnerADGuidName = "OwnerADGuid";

		public const string OwnerLegacyDNName = "OwnerLegacyDN";

		public const string MailboxInstanceGuidName = "MailboxInstanceGuid";

		public const string MappingSignatureGuidName = "MappingSignatureGuid";

		public const string MailboxOwnerDisplayNameName = "MailboxOwnerDisplayName";

		public const string DisplayNameName = "DisplayName";

		public const string SimpleDisplayNameName = "SimpleDisplayName";

		public const string CommentName = "Comment";

		public const string OofStateName = "OofState";

		public const string DeletedOnName = "DeletedOn";

		public const string StatusName = "Status";

		public const string LcidName = "Lcid";

		public const string PropertyBlobName = "PropertyBlob";

		public const string PropertyBagName = "PropertyBag";

		public const string LargePropertyValueBlobName = "LargePropertyValueBlob";

		public const string MessageCountName = "MessageCount";

		public const string MessageSizeName = "MessageSize";

		public const string HiddenMessageCountName = "HiddenMessageCount";

		public const string HiddenMessageSizeName = "HiddenMessageSize";

		public const string MessageDeletedCountName = "MessageDeletedCount";

		public const string MessageDeletedSizeName = "MessageDeletedSize";

		public const string HiddenMessageDeletedCountName = "HiddenMessageDeletedCount";

		public const string HiddenMessageDeletedSizeName = "HiddenMessageDeletedSize";

		public const string LastLogonTimeName = "LastLogonTime";

		public const string LastLogoffTimeName = "LastLogoffTime";

		public const string ConversationEnabledName = "ConversationEnabled";

		public const string MailboxDatabaseVersionName = "MailboxDatabaseVersion";

		public const string LastQuotaNotificationTimeName = "LastQuotaNotificationTime";

		public const string PreservingMailboxSignatureName = "PreservingMailboxSignature";

		public const string TenantHintName = "TenantHint";

		public const string NextMessageDocumentIdName = "NextMessageDocumentId";

		public const string DefaultPromotedMessagePropertyIdsName = "DefaultPromotedMessagePropertyIds";

		public const string AlwaysPromotedMessagePropertyIdsName = "AlwaysPromotedMessagePropertyIds";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string LastMailboxMaintenanceTimeName = "LastMailboxMaintenanceTime";

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string UnifiedMailboxGuidName = "UnifiedMailboxGuid";

		public const string PhysicalTableName = "Mailbox";

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn mailboxGuid;

		private PhysicalColumn ownerADGuid;

		private PhysicalColumn ownerLegacyDN;

		private PhysicalColumn mailboxInstanceGuid;

		private PhysicalColumn mappingSignatureGuid;

		private PhysicalColumn mailboxOwnerDisplayName;

		private PhysicalColumn displayName;

		private PhysicalColumn simpleDisplayName;

		private PhysicalColumn comment;

		private PhysicalColumn oofState;

		private PhysicalColumn deletedOn;

		private PhysicalColumn status;

		private PhysicalColumn lcid;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn propertyBag;

		private PhysicalColumn largePropertyValueBlob;

		private PhysicalColumn messageCount;

		private PhysicalColumn messageSize;

		private PhysicalColumn hiddenMessageCount;

		private PhysicalColumn hiddenMessageSize;

		private PhysicalColumn messageDeletedCount;

		private PhysicalColumn messageDeletedSize;

		private PhysicalColumn hiddenMessageDeletedCount;

		private PhysicalColumn hiddenMessageDeletedSize;

		private PhysicalColumn lastLogonTime;

		private PhysicalColumn lastLogoffTime;

		private PhysicalColumn conversationEnabled;

		private PhysicalColumn mailboxDatabaseVersion;

		private PhysicalColumn lastQuotaNotificationTime;

		private PhysicalColumn preservingMailboxSignature;

		private PhysicalColumn tenantHint;

		private PhysicalColumn nextMessageDocumentId;

		private PhysicalColumn defaultPromotedMessagePropertyIds;

		private PhysicalColumn alwaysPromotedMessagePropertyIds;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn lastMailboxMaintenanceTime;

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn unifiedMailboxGuid;

		private Index mailboxTablePK;

		private Index mailboxGuidIndex;

		private Index unifiedMailboxGuidIndex;

		private Table table;
	}
}
