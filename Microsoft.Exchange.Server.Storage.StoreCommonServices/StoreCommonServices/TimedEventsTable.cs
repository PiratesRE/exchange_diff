using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class TimedEventsTable
	{
		internal TimedEventsTable()
		{
			this.eventTime = Factory.CreatePhysicalColumn("EventTime", "EventTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.uniqueId = Factory.CreatePhysicalColumn("UniqueId", "UniqueId", typeof(long), false, true, false, false, false, Visibility.Public, 0, 8, 8);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.eventSource = Factory.CreatePhysicalColumn("EventSource", "EventSource", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.eventType = Factory.CreatePhysicalColumn("EventType", "EventType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.qoS = Factory.CreatePhysicalColumn("QoS", "QoS", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.eventData = Factory.CreatePhysicalColumn("EventData", "EventData", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "TimedEventsPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.timedEventsPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.EventTime,
				this.UniqueId
			});
			Index[] indexes = new Index[]
			{
				this.TimedEventsPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.EventTime,
				this.UniqueId,
				this.MailboxNumber,
				this.EventSource,
				this.EventType,
				this.QoS,
				this.EventData,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("TimedEvents", TableClass.TimedEvents, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn EventTime
		{
			get
			{
				return this.eventTime;
			}
		}

		public PhysicalColumn UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn EventSource
		{
			get
			{
				return this.eventSource;
			}
		}

		public PhysicalColumn EventType
		{
			get
			{
				return this.eventType;
			}
		}

		public PhysicalColumn QoS
		{
			get
			{
				return this.qoS;
			}
		}

		public PhysicalColumn EventData
		{
			get
			{
				return this.eventData;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index TimedEventsPK
		{
			get
			{
				return this.timedEventsPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.eventTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventTime = null;
			}
			physicalColumn = this.uniqueId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.uniqueId = null;
			}
			physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			physicalColumn = this.eventSource;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventSource = null;
			}
			physicalColumn = this.eventType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventType = null;
			}
			physicalColumn = this.qoS;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.qoS = null;
			}
			physicalColumn = this.eventData;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventData = null;
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
			Index index = this.timedEventsPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.timedEventsPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string EventTimeName = "EventTime";

		public const string UniqueIdName = "UniqueId";

		public const string MailboxNumberName = "MailboxNumber";

		public const string EventSourceName = "EventSource";

		public const string EventTypeName = "EventType";

		public const string QoSName = "QoS";

		public const string EventDataName = "EventData";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "TimedEvents";

		private PhysicalColumn eventTime;

		private PhysicalColumn uniqueId;

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn eventSource;

		private PhysicalColumn eventType;

		private PhysicalColumn qoS;

		private PhysicalColumn eventData;

		private PhysicalColumn extensionBlob;

		private Index timedEventsPK;

		private Table table;
	}
}
