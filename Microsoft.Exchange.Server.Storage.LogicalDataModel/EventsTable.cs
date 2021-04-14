using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class EventsTable
	{
		internal EventsTable()
		{
			this.eventCounter = Factory.CreatePhysicalColumn("EventCounter", "EventCounter", typeof(long), false, true, false, false, false, Visibility.Public, 0, 8, 8);
			this.createTime = Factory.CreatePhysicalColumn("CreateTime", "CreateTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.transactionId = Factory.CreatePhysicalColumn("TransactionId", "TransactionId", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.eventType = Factory.CreatePhysicalColumn("EventType", "EventType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.clientType = Factory.CreatePhysicalColumn("ClientType", "ClientType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.flags = Factory.CreatePhysicalColumn("Flags", "Flags", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.objectClass = Factory.CreatePhysicalColumn("ObjectClass", "ObjectClass", typeof(string), true, false, false, false, false, Visibility.Public, 59, 0, 59);
			this.fid = Factory.CreatePhysicalColumn("Fid", "Fid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.mid = Factory.CreatePhysicalColumn("Mid", "Mid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.oldFid = Factory.CreatePhysicalColumn("OldFid", "OldFid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.oldMid = Factory.CreatePhysicalColumn("OldMid", "OldMid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.oldParentFid = Factory.CreatePhysicalColumn("OldParentFid", "OldParentFid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.itemCount = Factory.CreatePhysicalColumn("ItemCount", "ItemCount", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.unreadCount = Factory.CreatePhysicalColumn("UnreadCount", "UnreadCount", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.sid = Factory.CreatePhysicalColumn("Sid", "Sid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 68, 0, 68);
			this.documentId = Factory.CreatePhysicalColumn("DocumentId", "DocumentId", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extendedFlags = Factory.CreatePhysicalColumn("ExtendedFlags", "ExtendedFlags", typeof(long), true, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.parentFid = Factory.CreatePhysicalColumn("ParentFid", "ParentFid", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 24, 24);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "EventsPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.eventsPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.EventCounter
			});
			Index[] indexes = new Index[]
			{
				this.EventsPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.EventCounter,
				this.CreateTime,
				this.TransactionId,
				this.EventType,
				this.MailboxNumber,
				this.ClientType,
				this.Flags,
				this.ObjectClass,
				this.Fid,
				this.Mid,
				this.OldFid,
				this.OldMid,
				this.OldParentFid,
				this.ItemCount,
				this.UnreadCount,
				this.Sid,
				this.DocumentId,
				this.ExtendedFlags,
				this.ParentFid,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("Events", TableClass.Events, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public PhysicalColumn CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		public PhysicalColumn TransactionId
		{
			get
			{
				return this.transactionId;
			}
		}

		public PhysicalColumn EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public PhysicalColumn Flags
		{
			get
			{
				return this.flags;
			}
		}

		public PhysicalColumn ObjectClass
		{
			get
			{
				return this.objectClass;
			}
		}

		public PhysicalColumn Fid
		{
			get
			{
				return this.fid;
			}
		}

		public PhysicalColumn Mid
		{
			get
			{
				return this.mid;
			}
		}

		public PhysicalColumn OldFid
		{
			get
			{
				return this.oldFid;
			}
		}

		public PhysicalColumn OldMid
		{
			get
			{
				return this.oldMid;
			}
		}

		public PhysicalColumn OldParentFid
		{
			get
			{
				return this.oldParentFid;
			}
		}

		public PhysicalColumn ItemCount
		{
			get
			{
				return this.itemCount;
			}
		}

		public PhysicalColumn UnreadCount
		{
			get
			{
				return this.unreadCount;
			}
		}

		public PhysicalColumn Sid
		{
			get
			{
				return this.sid;
			}
		}

		public PhysicalColumn DocumentId
		{
			get
			{
				return this.documentId;
			}
		}

		public PhysicalColumn ExtendedFlags
		{
			get
			{
				return this.extendedFlags;
			}
		}

		public PhysicalColumn ParentFid
		{
			get
			{
				return this.parentFid;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index EventsPK
		{
			get
			{
				return this.eventsPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.eventCounter;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventCounter = null;
			}
			physicalColumn = this.createTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.createTime = null;
			}
			physicalColumn = this.transactionId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.transactionId = null;
			}
			physicalColumn = this.eventType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventType = null;
			}
			physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			physicalColumn = this.clientType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.clientType = null;
			}
			physicalColumn = this.flags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.flags = null;
			}
			physicalColumn = this.objectClass;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.objectClass = null;
			}
			physicalColumn = this.fid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.fid = null;
			}
			physicalColumn = this.mid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mid = null;
			}
			physicalColumn = this.oldFid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.oldFid = null;
			}
			physicalColumn = this.oldMid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.oldMid = null;
			}
			physicalColumn = this.oldParentFid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.oldParentFid = null;
			}
			physicalColumn = this.itemCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.itemCount = null;
			}
			physicalColumn = this.unreadCount;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.unreadCount = null;
			}
			physicalColumn = this.sid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.sid = null;
			}
			physicalColumn = this.documentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.documentId = null;
			}
			physicalColumn = this.extendedFlags;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extendedFlags = null;
			}
			physicalColumn = this.parentFid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.parentFid = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.eventsPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.eventsPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string EventCounterName = "EventCounter";

		public const string CreateTimeName = "CreateTime";

		public const string TransactionIdName = "TransactionId";

		public const string EventTypeName = "EventType";

		public const string MailboxNumberName = "MailboxNumber";

		public const string ClientTypeName = "ClientType";

		public const string FlagsName = "Flags";

		public const string ObjectClassName = "ObjectClass";

		public const string FidName = "Fid";

		public const string MidName = "Mid";

		public const string OldFidName = "OldFid";

		public const string OldMidName = "OldMid";

		public const string OldParentFidName = "OldParentFid";

		public const string ItemCountName = "ItemCount";

		public const string UnreadCountName = "UnreadCount";

		public const string SidName = "Sid";

		public const string DocumentIdName = "DocumentId";

		public const string ExtendedFlagsName = "ExtendedFlags";

		public const string ParentFidName = "ParentFid";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "Events";

		private PhysicalColumn eventCounter;

		private PhysicalColumn createTime;

		private PhysicalColumn transactionId;

		private PhysicalColumn eventType;

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn clientType;

		private PhysicalColumn flags;

		private PhysicalColumn objectClass;

		private PhysicalColumn fid;

		private PhysicalColumn mid;

		private PhysicalColumn oldFid;

		private PhysicalColumn oldMid;

		private PhysicalColumn oldParentFid;

		private PhysicalColumn itemCount;

		private PhysicalColumn unreadCount;

		private PhysicalColumn sid;

		private PhysicalColumn documentId;

		private PhysicalColumn extendedFlags;

		private PhysicalColumn parentFid;

		private PhysicalColumn extensionBlob;

		private Index eventsPK;

		private Table table;
	}
}
