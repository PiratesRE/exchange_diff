using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class MailboxIdentityTable
	{
		internal MailboxIdentityTable()
		{
			this.mailboxPartitionNumber = Factory.CreatePhysicalColumn("MailboxPartitionNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.localIdGuid = Factory.CreatePhysicalColumn("LocalIdGuid", "LocalIdGuid", typeof(Guid), false, false, false, false, false, Visibility.Public, 0, 16, 16);
			this.idCounter = Factory.CreatePhysicalColumn("IdCounter", "IdCounter", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.cnCounter = Factory.CreatePhysicalColumn("CnCounter", "CnCounter", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.lastCounterPatchingTime = Factory.CreatePhysicalColumn("LastCounterPatchingTime", "LastCounterPatchingTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			this.nextMessageDocumentId = Factory.CreatePhysicalColumn("NextMessageDocumentId", "NextMessageDocumentId", typeof(int), true, false, false, false, true, Visibility.Public, 0, 4, 4);
			string name = "MailboxIdentityPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[1];
			this.mailboxIdentityPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true
			}, new PhysicalColumn[]
			{
				this.MailboxPartitionNumber
			});
			Index[] indexes = new Index[]
			{
				this.MailboxIdentityPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxPartitionNumber,
				this.LocalIdGuid,
				this.IdCounter,
				this.CnCounter,
				this.LastCounterPatchingTime,
				this.ExtensionBlob,
				this.NextMessageDocumentId
			};
			this.table = Factory.CreateTable("MailboxIdentity", TableClass.MailboxIdentity, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn LocalIdGuid
		{
			get
			{
				return this.localIdGuid;
			}
		}

		public PhysicalColumn IdCounter
		{
			get
			{
				return this.idCounter;
			}
		}

		public PhysicalColumn CnCounter
		{
			get
			{
				return this.cnCounter;
			}
		}

		public PhysicalColumn LastCounterPatchingTime
		{
			get
			{
				return this.lastCounterPatchingTime;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public PhysicalColumn NextMessageDocumentId
		{
			get
			{
				return this.nextMessageDocumentId;
			}
		}

		public Index MailboxIdentityPK
		{
			get
			{
				return this.mailboxIdentityPK;
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
			physicalColumn = this.localIdGuid;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.localIdGuid = null;
			}
			physicalColumn = this.idCounter;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.idCounter = null;
			}
			physicalColumn = this.cnCounter;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.cnCounter = null;
			}
			physicalColumn = this.lastCounterPatchingTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.lastCounterPatchingTime = null;
			}
			physicalColumn = this.extensionBlob;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.extensionBlob = null;
			}
			physicalColumn = this.nextMessageDocumentId;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.nextMessageDocumentId = null;
			}
			for (int i = this.Table.Columns.Count - 1; i >= 0; i--)
			{
				this.Table.Columns[i].Index = i;
			}
			Index index = this.mailboxIdentityPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.mailboxIdentityPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxPartitionNumberName = "MailboxPartitionNumber";

		public const string LocalIdGuidName = "LocalIdGuid";

		public const string IdCounterName = "IdCounter";

		public const string CnCounterName = "CnCounter";

		public const string LastCounterPatchingTimeName = "LastCounterPatchingTime";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string NextMessageDocumentIdName = "NextMessageDocumentId";

		public const string PhysicalTableName = "MailboxIdentity";

		private PhysicalColumn mailboxPartitionNumber;

		private PhysicalColumn localIdGuid;

		private PhysicalColumn idCounter;

		private PhysicalColumn cnCounter;

		private PhysicalColumn lastCounterPatchingTime;

		private PhysicalColumn extensionBlob;

		private PhysicalColumn nextMessageDocumentId;

		private Index mailboxIdentityPK;

		private Table table;
	}
}
