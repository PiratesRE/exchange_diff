using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class TombstoneTable
	{
		internal TombstoneTable()
		{
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.inid = Factory.CreatePhysicalColumn("Inid", "Inid", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.sizeEstimate = Factory.CreatePhysicalColumn("SizeEstimate", "SizeEstimate", typeof(long), true, false, false, false, true, Visibility.Public, 0, 8, 8);
			this.clientType = Factory.CreatePhysicalColumn("ClientType", "ClientType", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string name = "TombstonePK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.tombstonePK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.Inid
			});
			Index[] indexes = new Index[]
			{
				this.TombstonePK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.Inid,
				this.ExtensionBlob,
				this.SizeEstimate,
				this.ClientType
			};
			this.table = Factory.CreateTable("Tombstone", TableClass.Tombstone, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn Inid
		{
			get
			{
				return this.inid;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn SizeEstimate
		{
			get
			{
				return this.sizeEstimate;
			}
		}

		public PhysicalColumn ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public Index TombstonePK
		{
			get
			{
				return this.tombstonePK;
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
			physicalColumn = this.inid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.inid = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.sizeEstimate;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.sizeEstimate = null;
			}
			physicalColumn = this.clientType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.clientType = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.tombstonePK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.tombstonePK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxNumberName = "MailboxNumber";

		public const string InidName = "Inid";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string SizeEstimateName = "SizeEstimate";

		public const string ClientTypeName = "ClientType";

		public const string PhysicalTableName = "Tombstone";

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn inid;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn sizeEstimate;

		private PhysicalColumn clientType;

		private Index tombstonePK;

		private Table table;
	}
}
