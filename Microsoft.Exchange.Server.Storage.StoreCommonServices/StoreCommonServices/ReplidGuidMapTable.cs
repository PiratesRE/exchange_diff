using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class ReplidGuidMapTable
	{
		internal ReplidGuidMapTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.guid = Factory.CreatePhysicalColumn("Guid", "Guid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.replid = Factory.CreatePhysicalColumn("Replid", "Replid", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "ReplidGuidMapPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.replidGuidMapPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.Guid
			});
			Index[] indexes = new Index[]
			{
				this.ReplidGuidMapPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.Guid,
				this.Replid,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("ReplidGuidMap", TableClass.ReplidGuidMap, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn Guid
		{
			get
			{
				return this.guid;
			}
		}

		public PhysicalColumn Replid
		{
			get
			{
				return this.replid;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index ReplidGuidMapPK
		{
			get
			{
				return this.replidGuidMapPK;
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
			physicalColumn = this.guid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.guid = null;
			}
			physicalColumn = this.replid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.replid = null;
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
			Index index = this.replidGuidMapPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.replidGuidMapPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string GuidName = "Guid";

		public const string ReplidName = "Replid";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "ReplidGuidMap";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn guid;

		private PhysicalColumn replid;

		private PhysicalColumn extensionBlob;

		private Index replidGuidMapPK;

		private Table table;
	}
}
