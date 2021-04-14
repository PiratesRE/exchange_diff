using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class PseudoIndexMaintenanceTable
	{
		internal PseudoIndexMaintenanceTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.updateRecordNumber = Factory.CreatePhysicalColumn("UpdateRecordNumber", "UpdateRecordNumber", typeof(long), false, true, false, false, false, Visibility.Public, 0, 8, 8);
			this.logicalIndexNumber = Factory.CreatePhysicalColumn("LogicalIndexNumber", "LogicalIndexNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.logicalOperation = Factory.CreatePhysicalColumn("LogicalOperation", "LogicalOperation", typeof(short), false, false, false, false, false, Visibility.Public, 0, 2, 2);
			this.updatedPropertiesBlob = Factory.CreatePhysicalColumn("UpdatedPropertiesBlob", "UpdatedPropertiesBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "PseudoIndexMaintenancePK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.pseudoIndexMaintenancePK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.UpdateRecordNumber
			});
			Index[] indexes = new Index[]
			{
				this.PseudoIndexMaintenancePK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.UpdateRecordNumber,
				this.LogicalIndexNumber,
				this.LogicalOperation,
				this.UpdatedPropertiesBlob,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("PseudoIndexMaintenance", TableClass.PseudoIndexMaintenance, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Redacted, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn UpdateRecordNumber
		{
			get
			{
				return this.updateRecordNumber;
			}
		}

		public PhysicalColumn LogicalIndexNumber
		{
			get
			{
				return this.logicalIndexNumber;
			}
		}

		public PhysicalColumn LogicalOperation
		{
			get
			{
				return this.logicalOperation;
			}
		}

		public PhysicalColumn UpdatedPropertiesBlob
		{
			get
			{
				return this.updatedPropertiesBlob;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index PseudoIndexMaintenancePK
		{
			get
			{
				return this.pseudoIndexMaintenancePK;
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
			physicalColumn = this.updateRecordNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.updateRecordNumber = null;
			}
			physicalColumn = this.logicalIndexNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.logicalIndexNumber = null;
			}
			physicalColumn = this.logicalOperation;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.logicalOperation = null;
			}
			physicalColumn = this.updatedPropertiesBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.updatedPropertiesBlob = null;
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
			Index index = this.pseudoIndexMaintenancePK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.pseudoIndexMaintenancePK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string UpdateRecordNumberName = "UpdateRecordNumber";

		public const string LogicalIndexNumberName = "LogicalIndexNumber";

		public const string LogicalOperationName = "LogicalOperation";

		public const string UpdatedPropertiesBlobName = "UpdatedPropertiesBlob";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "PseudoIndexMaintenance";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn updateRecordNumber;

		private PhysicalColumn logicalIndexNumber;

		private PhysicalColumn logicalOperation;

		private PhysicalColumn updatedPropertiesBlob;

		private PhysicalColumn extensionBlob;

		private Index pseudoIndexMaintenancePK;

		private Table table;
	}
}
