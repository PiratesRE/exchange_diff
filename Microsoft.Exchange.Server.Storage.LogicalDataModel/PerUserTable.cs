using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class PerUserTable
	{
		internal PerUserTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxPartitionNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.residentFolder = Factory.CreatePhysicalColumn("ResidentFolder", "ResidentFolder", typeof(bool), false, false, false, false, false, Visibility.Public, 0, 1, 1);
			this.guid = Factory.CreatePhysicalColumn("Guid", "Guid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.folderId = Factory.CreatePhysicalColumn("FolderId", "FolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 26, 0, 26);
			this.cnsetRead = Factory.CreatePhysicalColumn("CnsetRead", "CnsetRead", typeof(byte[]), false, false, false, false, false, Visibility.Public, 1048576, 0, 1048576);
			this.lastModificationTime = Factory.CreatePhysicalColumn("LastModificationTime", "LastModificationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "PerUserPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[4];
			this.perUserPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.ResidentFolder,
				this.FolderId,
				this.Guid
			});
			Index[] indexes = new Index[]
			{
				this.PerUserPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 1);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.ResidentFolder,
				this.Guid,
				this.FolderId,
				this.CnsetRead,
				this.LastModificationTime,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("PerUser", TableClass.PerUser, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn ResidentFolder
		{
			get
			{
				return this.residentFolder;
			}
		}

		public PhysicalColumn Guid
		{
			get
			{
				return this.guid;
			}
		}

		public PhysicalColumn FolderId
		{
			get
			{
				return this.folderId;
			}
		}

		public PhysicalColumn CnsetRead
		{
			get
			{
				return this.cnsetRead;
			}
		}

		public PhysicalColumn LastModificationTime
		{
			get
			{
				return this.lastModificationTime;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index PerUserPK
		{
			get
			{
				return this.perUserPK;
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
			physicalColumn = this.residentFolder;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.residentFolder = null;
			}
			physicalColumn = this.guid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.guid = null;
			}
			physicalColumn = this.folderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.folderId = null;
			}
			physicalColumn = this.cnsetRead;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.cnsetRead = null;
			}
			physicalColumn = this.lastModificationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastModificationTime = null;
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
			Index index = this.perUserPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.perUserPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string ResidentFolderName = "ResidentFolder";

		public const string GuidName = "Guid";

		public const string FolderIdName = "FolderId";

		public const string CnsetReadName = "CnsetRead";

		public const string LastModificationTimeName = "LastModificationTime";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "PerUser";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn residentFolder;

		private PhysicalColumn guid;

		private PhysicalColumn folderId;

		private PhysicalColumn cnsetRead;

		private PhysicalColumn lastModificationTime;

		private PhysicalColumn extensionBlob;

		private Index perUserPK;

		private Table table;
	}
}
