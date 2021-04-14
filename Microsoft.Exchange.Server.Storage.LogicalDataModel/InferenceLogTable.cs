using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class InferenceLogTable
	{
		internal InferenceLogTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.rowId = Factory.CreatePhysicalColumn("RowId", "RowId", typeof(int), false, true, false, false, false, Visibility.Public, 0, 4, 4);
			this.createTime = Factory.CreatePhysicalColumn("CreateTime", "CreateTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.propertyBlob = Factory.CreatePhysicalColumn("PropertyBlob", "PropertyBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 4096, 0, 4096);
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string name = "InferenceLogPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.inferenceLogPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.RowId
			});
			Index[] indexes = new Index[]
			{
				this.InferenceLogPK
			};
			SpecialColumns specialCols = new SpecialColumns(this.PropertyBlob, null, null, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.RowId,
				this.CreateTime,
				this.PropertyBlob,
				this.MailboxNumber
			};
			this.table = Factory.CreateTable("InferenceLog", TableClass.InferenceLog, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Redacted, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn RowId
		{
			get
			{
				return this.rowId;
			}
		}

		public PhysicalColumn CreateTime
		{
			get
			{
				return this.createTime;
			}
		}

		public PhysicalColumn PropertyBlob
		{
			get
			{
				return this.propertyBlob;
			}
		}

		public PhysicalColumn MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		public Index InferenceLogPK
		{
			get
			{
				return this.inferenceLogPK;
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
			physicalColumn = this.rowId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.rowId = null;
			}
			physicalColumn = this.createTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.createTime = null;
			}
			physicalColumn = this.propertyBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.propertyBlob = null;
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
			Index index = this.inferenceLogPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.inferenceLogPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string RowIdName = "RowId";

		public const string CreateTimeName = "CreateTime";

		public const string PropertyBlobName = "PropertyBlob";

		public const string MailboxNumberName = "MailboxNumber";

		public const string PhysicalTableName = "InferenceLog";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn rowId;

		private PhysicalColumn createTime;

		private PhysicalColumn propertyBlob;

		private PhysicalColumn mailboxNumber;

		private Index inferenceLogPK;

		private Table table;
	}
}
