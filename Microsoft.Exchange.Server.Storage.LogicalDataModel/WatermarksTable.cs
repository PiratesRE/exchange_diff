using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class WatermarksTable
	{
		internal WatermarksTable()
		{
			this.consumerGuid = Factory.CreatePhysicalColumn("ConsumerGuid", "ConsumerGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.eventCounter = Factory.CreatePhysicalColumn("EventCounter", "EventCounter", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "WatermarksPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.watermarksPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.ConsumerGuid,
				this.MailboxNumber
			});
			Index[] indexes = new Index[]
			{
				this.WatermarksPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.ConsumerGuid,
				this.MailboxNumber,
				this.EventCounter,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("Watermarks", TableClass.Watermarks, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
		}

		public Table Table
		{
			get
			{
				return this.table;
			}
		}

		public PhysicalColumn ConsumerGuid
		{
			get
			{
				return this.consumerGuid;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public PhysicalColumn EventCounter
		{
			get
			{
				return this.eventCounter;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index WatermarksPK
		{
			get
			{
				return this.watermarksPK;
			}
		}

		internal void PostMountInitialize(ComponentVersion databaseVersion)
		{
			PhysicalColumn physicalColumn = this.consumerGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.consumerGuid = null;
			}
			physicalColumn = this.mailboxNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.mailboxNumber = null;
			}
			physicalColumn = this.eventCounter;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.eventCounter = null;
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
			Index index = this.watermarksPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.watermarksPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string ConsumerGuidName = "ConsumerGuid";

		public const string MailboxNumberName = "MailboxNumber";

		public const string EventCounterName = "EventCounter";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "Watermarks";

		private PhysicalColumn consumerGuid;

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn eventCounter;

		private PhysicalColumn extensionBlob;

		private Index watermarksPK;

		private Table table;
	}
}
