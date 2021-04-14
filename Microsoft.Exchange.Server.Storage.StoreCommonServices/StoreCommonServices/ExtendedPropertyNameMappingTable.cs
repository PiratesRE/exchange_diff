using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class ExtendedPropertyNameMappingTable
	{
		internal ExtendedPropertyNameMappingTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.propNumber = Factory.CreatePhysicalColumn("PropNumber", "PropNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.propGuid = Factory.CreatePhysicalColumn("PropGuid", "PropGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.propName = Factory.CreatePhysicalColumn("PropName", "PropName", typeof(string), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.propDispId = Factory.CreatePhysicalColumn("PropDispId", "PropDispId", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "ExtendedPropertyNameMappingPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.extendedPropertyNameMappingPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.PropNumber
			});
			Index[] indexes = new Index[]
			{
				this.ExtendedPropertyNameMappingPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.PropNumber,
				this.PropGuid,
				this.PropName,
				this.PropDispId,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("ExtendedPropertyNameMapping", TableClass.ExtendedPropertyNameMapping, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn PropNumber
		{
			get
			{
				return this.propNumber;
			}
		}

		public PhysicalColumn PropGuid
		{
			get
			{
				return this.propGuid;
			}
		}

		public PhysicalColumn PropName
		{
			get
			{
				return this.propName;
			}
		}

		public PhysicalColumn PropDispId
		{
			get
			{
				return this.propDispId;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index ExtendedPropertyNameMappingPK
		{
			get
			{
				return this.extendedPropertyNameMappingPK;
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
			physicalColumn = this.propNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propNumber = null;
			}
			physicalColumn = this.propGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propGuid = null;
			}
			physicalColumn = this.propName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propName = null;
			}
			physicalColumn = this.propDispId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propDispId = null;
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
			Index index = this.extendedPropertyNameMappingPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.extendedPropertyNameMappingPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string PropNumberName = "PropNumber";

		public const string PropGuidName = "PropGuid";

		public const string PropNameName = "PropName";

		public const string PropDispIdName = "PropDispId";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "ExtendedPropertyNameMapping";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn propNumber;

		private PhysicalColumn propGuid;

		private PhysicalColumn propName;

		private PhysicalColumn propDispId;

		private PhysicalColumn extensionBlob;

		private Index extendedPropertyNameMappingPK;

		private Table table;
	}
}
