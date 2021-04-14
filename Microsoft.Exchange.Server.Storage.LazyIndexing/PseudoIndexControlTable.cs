using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public sealed class PseudoIndexControlTable
	{
		internal PseudoIndexControlTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.logicalIndexNumber = Factory.CreatePhysicalColumn("LogicalIndexNumber", "LogicalIndexNumber", typeof(int), false, true, false, false, false, Visibility.Public, 0, 4, 4);
			this.indexType = Factory.CreatePhysicalColumn("IndexType", "IndexType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.physicalIndexNumber = Factory.CreatePhysicalColumn("PhysicalIndexNumber", "PhysicalIndexNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.firstUpdateRecord = Factory.CreatePhysicalColumn("FirstUpdateRecord", "FirstUpdateRecord", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastReferenceDate = Factory.CreatePhysicalColumn("LastReferenceDate", "LastReferenceDate", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.columnMappings = Factory.CreatePhysicalColumn("ColumnMappings", "ColumnMappings", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.conditionalIndex = Factory.CreatePhysicalColumn("ConditionalIndex", "ConditionalIndex", typeof(byte[]), true, false, false, false, false, Visibility.Public, 1073741824, 0, 1073741824);
			this.tableName = Factory.CreatePhysicalColumn("TableName", "TableName", typeof(string), false, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.categorizationInfo = Factory.CreatePhysicalColumn("CategorizationInfo", "CategorizationInfo", typeof(byte[]), true, false, false, false, false, Visibility.Public, 256, 0, 256);
			this.logicalIndexVersion = Factory.CreatePhysicalColumn("LogicalIndexVersion", "LogicalIndexVersion", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.indexSignature = Factory.CreatePhysicalColumn("IndexSignature", "IndexSignature", typeof(int), true, false, false, false, false, Visibility.Public, 0, 4, 4);
			string name = "PseudoIndexControlPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[3];
			this.pseudoIndexControlPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.LogicalIndexNumber
			});
			Index[] indexes = new Index[]
			{
				this.PseudoIndexControlPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.FolderId,
				this.LogicalIndexNumber,
				this.IndexType,
				this.PhysicalIndexNumber,
				this.FirstUpdateRecord,
				this.LastReferenceDate,
				this.ColumnMappings,
				this.ConditionalIndex,
				this.TableName,
				this.CategorizationInfo,
				this.LogicalIndexVersion,
				this.ExtensionBlob,
				this.IndexSignature
			};
			this.table = Factory.CreateTable("PseudoIndexControl", TableClass.PseudoIndexControl, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public PhysicalColumn LogicalIndexNumber
		{
			get
			{
				return this.logicalIndexNumber;
			}
		}

		public PhysicalColumn IndexType
		{
			get
			{
				return this.indexType;
			}
		}

		public PhysicalColumn PhysicalIndexNumber
		{
			get
			{
				return this.physicalIndexNumber;
			}
		}

		public PhysicalColumn FirstUpdateRecord
		{
			get
			{
				return this.firstUpdateRecord;
			}
		}

		public PhysicalColumn LastReferenceDate
		{
			get
			{
				return this.lastReferenceDate;
			}
		}

		public PhysicalColumn ColumnMappings
		{
			get
			{
				return this.columnMappings;
			}
		}

		public PhysicalColumn ConditionalIndex
		{
			get
			{
				return this.conditionalIndex;
			}
		}

		public PhysicalColumn TableName
		{
			get
			{
				return this.tableName;
			}
		}

		public PhysicalColumn CategorizationInfo
		{
			get
			{
				return this.categorizationInfo;
			}
		}

		public PhysicalColumn LogicalIndexVersion
		{
			get
			{
				return this.logicalIndexVersion;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn IndexSignature
		{
			get
			{
				return this.indexSignature;
			}
		}

		public Index PseudoIndexControlPK
		{
			get
			{
				return this.pseudoIndexControlPK;
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
			physicalColumn = this.folderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.folderId = null;
			}
			physicalColumn = this.logicalIndexNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.logicalIndexNumber = null;
			}
			physicalColumn = this.indexType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.indexType = null;
			}
			physicalColumn = this.physicalIndexNumber;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.physicalIndexNumber = null;
			}
			physicalColumn = this.firstUpdateRecord;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.firstUpdateRecord = null;
			}
			physicalColumn = this.lastReferenceDate;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastReferenceDate = null;
			}
			physicalColumn = this.columnMappings;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.columnMappings = null;
			}
			physicalColumn = this.conditionalIndex;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.conditionalIndex = null;
			}
			physicalColumn = this.tableName;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.tableName = null;
			}
			physicalColumn = this.categorizationInfo;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.categorizationInfo = null;
			}
			physicalColumn = this.logicalIndexVersion;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.logicalIndexVersion = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.indexSignature;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.indexSignature = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.pseudoIndexControlPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.pseudoIndexControlPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string FolderIdName = "FolderId";

		public const string LogicalIndexNumberName = "LogicalIndexNumber";

		public const string IndexTypeName = "IndexType";

		public const string PhysicalIndexNumberName = "PhysicalIndexNumber";

		public const string FirstUpdateRecordName = "FirstUpdateRecord";

		public const string LastReferenceDateName = "LastReferenceDate";

		public const string ColumnMappingsName = "ColumnMappings";

		public const string ConditionalIndexName = "ConditionalIndex";

		public const string TableNameName = "TableName";

		public const string CategorizationInfoName = "CategorizationInfo";

		public const string LogicalIndexVersionName = "LogicalIndexVersion";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string IndexSignatureName = "IndexSignature";

		public const string PhysicalTableName = "PseudoIndexControl";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn folderId;

		private PhysicalColumn logicalIndexNumber;

		private PhysicalColumn indexType;

		private PhysicalColumn physicalIndexNumber;

		private PhysicalColumn firstUpdateRecord;

		private PhysicalColumn lastReferenceDate;

		private PhysicalColumn columnMappings;

		private PhysicalColumn conditionalIndex;

		private PhysicalColumn tableName;

		private PhysicalColumn categorizationInfo;

		private PhysicalColumn logicalIndexVersion;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn indexSignature;

		private Index pseudoIndexControlPK;

		private Table table;
	}
}
