using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class SearchQueueTable
	{
		internal SearchQueueTable()
		{
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.searchFolderId = Factory.CreatePhysicalColumn("SearchFolderId", "SearchFolderId", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.userSid = Factory.CreatePhysicalColumn("UserSid", "UserSid", typeof(byte[]), false, false, false, false, false, Visibility.Public, 0, 512, 512);
			this.clientType = Factory.CreatePhysicalColumn("ClientType", "ClientType", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.lCID = Factory.CreatePhysicalColumn("LCID", "LCID", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.incrementalScopeFolderId = Factory.CreatePhysicalColumn("IncrementalScopeFolderId", "IncrementalScopeFolderId", typeof(byte[]), true, false, false, false, false, Visibility.Public, 0, 26, 26);
			this.creationTime = Factory.CreatePhysicalColumn("CreationTime", "CreationTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.populationAttempts = Factory.CreatePhysicalColumn("PopulationAttempts", "PopulationAttempts", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "SearchQueuePK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[2];
			this.searchQueuePK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true
			}, new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.SearchFolderId
			});
			Index[] indexes = new Index[]
			{
				this.SearchQueuePK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.SearchFolderId,
				this.UserSid,
				this.ClientType,
				this.LCID,
				this.IncrementalScopeFolderId,
				this.CreationTime,
				this.PopulationAttempts,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("SearchQueue", TableClass.SearchQueue, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn SearchFolderId
		{
			get
			{
				return this.searchFolderId;
			}
		}

		public PhysicalColumn UserSid
		{
			get
			{
				return this.userSid;
			}
		}

		public PhysicalColumn ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		public PhysicalColumn LCID
		{
			get
			{
				return this.lCID;
			}
		}

		public PhysicalColumn IncrementalScopeFolderId
		{
			get
			{
				return this.incrementalScopeFolderId;
			}
		}

		public PhysicalColumn CreationTime
		{
			get
			{
				return this.creationTime;
			}
		}

		public PhysicalColumn PopulationAttempts
		{
			get
			{
				return this.populationAttempts;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index SearchQueuePK
		{
			get
			{
				return this.searchQueuePK;
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
			physicalColumn = this.searchFolderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.searchFolderId = null;
			}
			physicalColumn = this.userSid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.userSid = null;
			}
			physicalColumn = this.clientType;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.clientType = null;
			}
			physicalColumn = this.lCID;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lCID = null;
			}
			physicalColumn = this.incrementalScopeFolderId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.incrementalScopeFolderId = null;
			}
			physicalColumn = this.creationTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.creationTime = null;
			}
			physicalColumn = this.populationAttempts;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.populationAttempts = null;
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
			Index index = this.searchQueuePK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.searchQueuePK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxNumberName = "MailboxNumber";

		public const string SearchFolderIdName = "SearchFolderId";

		public const string UserSidName = "UserSid";

		public const string ClientTypeName = "ClientType";

		public const string LCIDName = "LCID";

		public const string IncrementalScopeFolderIdName = "IncrementalScopeFolderId";

		public const string CreationTimeName = "CreationTime";

		public const string PopulationAttemptsName = "PopulationAttempts";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "SearchQueue";

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn searchFolderId;

		private PhysicalColumn userSid;

		private PhysicalColumn clientType;

		private PhysicalColumn lCID;

		private PhysicalColumn incrementalScopeFolderId;

		private PhysicalColumn creationTime;

		private PhysicalColumn populationAttempts;

		private PhysicalColumn extensionBlob;

		private Index searchQueuePK;

		private Table table;
	}
}
