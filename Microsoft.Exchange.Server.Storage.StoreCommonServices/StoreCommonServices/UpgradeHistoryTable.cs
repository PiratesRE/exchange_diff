using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public sealed class UpgradeHistoryTable
	{
		internal UpgradeHistoryTable()
		{
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.upgradeTime = Factory.CreatePhysicalColumn("UpgradeTime", "UpgradeTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.fromVersion = Factory.CreatePhysicalColumn("FromVersion", "FromVersion", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.toVersion = Factory.CreatePhysicalColumn("ToVersion", "ToVersion", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "UpgradeHistoryPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[3];
			this.upgradeHistoryPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.UpgradeTime,
				this.FromVersion,
				this.MailboxNumber
			});
			Index[] indexes = new Index[]
			{
				this.UpgradeHistoryPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.UpgradeTime,
				this.FromVersion,
				this.ToVersion,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("UpgradeHistory", TableClass.UpgradeHistory, CultureHelper.DefaultCultureInfo, true, TableAccessHints.None, false, Visibility.Public, true, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn UpgradeTime
		{
			get
			{
				return this.upgradeTime;
			}
		}

		public PhysicalColumn FromVersion
		{
			get
			{
				return this.fromVersion;
			}
		}

		public PhysicalColumn ToVersion
		{
			get
			{
				return this.toVersion;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index UpgradeHistoryPK
		{
			get
			{
				return this.upgradeHistoryPK;
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
			physicalColumn = this.upgradeTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.upgradeTime = null;
			}
			physicalColumn = this.fromVersion;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.fromVersion = null;
			}
			physicalColumn = this.toVersion;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.toVersion = null;
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
			Index index = this.upgradeHistoryPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.upgradeHistoryPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxNumberName = "MailboxNumber";

		public const string UpgradeTimeName = "UpgradeTime";

		public const string FromVersionName = "FromVersion";

		public const string ToVersionName = "ToVersion";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "UpgradeHistory";

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn upgradeTime;

		private PhysicalColumn fromVersion;

		private PhysicalColumn toVersion;

		private PhysicalColumn extensionBlob;

		private Index upgradeHistoryPK;

		private Table table;
	}
}
