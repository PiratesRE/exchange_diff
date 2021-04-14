using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class DeliveredToTable
	{
		internal DeliveredToTable()
		{
			this.mailboxNumber = Factory.CreatePhysicalColumn("MailboxNumber", "MailboxNumber", typeof(int), false, false, false, false, false, Visibility.Public, 0, 4, 4);
			this.submitTime = Factory.CreatePhysicalColumn("SubmitTime", "SubmitTime", typeof(DateTime), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.messageIdHash = Factory.CreatePhysicalColumn("MessageIdHash", "MessageIdHash", typeof(long), false, false, false, false, false, Visibility.Public, 0, 8, 8);
			this.extensionBlob = Factory.CreatePhysicalColumn("ExtensionBlob", "ExtensionBlob", typeof(byte[]), true, false, false, false, false, Visibility.Redacted, 1073741824, 0, 1073741824);
			string name = "DeliveredToPK";
			bool primaryKey = true;
			bool unique = true;
			bool schemaExtension = false;
			bool[] conditional = new bool[3];
			this.deliveredToPK = new Index(name, primaryKey, unique, schemaExtension, conditional, new bool[]
			{
				true,
				true,
				true
			}, new PhysicalColumn[]
			{
				this.SubmitTime,
				this.MailboxNumber,
				this.MessageIdHash
			});
			Index[] indexes = new Index[]
			{
				this.DeliveredToPK
			};
			SpecialColumns specialCols = new SpecialColumns(null, null, null, 0);
			PhysicalColumn[] computedColumns = new PhysicalColumn[0];
			PhysicalColumn[] columns = new PhysicalColumn[]
			{
				this.MailboxNumber,
				this.SubmitTime,
				this.MessageIdHash,
				this.ExtensionBlob
			};
			this.table = Factory.CreateTable("DeliveredTo", TableClass.DeliveredTo, CultureHelper.DefaultCultureInfo, false, TableAccessHints.None, false, Visibility.Public, false, specialCols, indexes, computedColumns, columns);
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

		public PhysicalColumn SubmitTime
		{
			get
			{
				return this.submitTime;
			}
		}

		public PhysicalColumn MessageIdHash
		{
			get
			{
				return this.messageIdHash;
			}
		}

		public PhysicalColumn ExtensionBlob
		{
			get
			{
				return this.extensionBlob;
			}
		}

		public Index DeliveredToPK
		{
			get
			{
				return this.deliveredToPK;
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
			physicalColumn = this.submitTime;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.submitTime = null;
			}
			physicalColumn = this.messageIdHash;
			if (physicalColumn.MinVersion > databaseVersion.Value || databaseVersion.Value > physicalColumn.MaxVersion)
			{
				this.Table.Columns.Remove(physicalColumn);
				this.Table.CommonColumns.Remove(physicalColumn);
				this.messageIdHash = null;
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
			Index index = this.deliveredToPK;
			if (index.MinVersion > databaseVersion.Value || databaseVersion.Value > index.MaxVersion)
			{
				this.deliveredToPK = null;
				this.Table.Indexes.Remove(index);
			}
		}

		public const string MailboxNumberName = "MailboxNumber";

		public const string SubmitTimeName = "SubmitTime";

		public const string MessageIdHashName = "MessageIdHash";

		public const string ExtensionBlobName = "ExtensionBlob";

		public const string PhysicalTableName = "DeliveredTo";

		private PhysicalColumn mailboxNumber;

		private PhysicalColumn submitTime;

		private PhysicalColumn messageIdHash;

		private PhysicalColumn extensionBlob;

		private Index deliveredToPK;

		private Table table;
	}
}
