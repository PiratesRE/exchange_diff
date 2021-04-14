using System;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Migration
{
	internal class PublicFolderMigrationDataRow : IMigrationDataRow
	{
		public PublicFolderMigrationDataRow(int rowIndex, string mailboxName)
		{
			this.LocalMailboxIdentifier = mailboxName;
			this.Identifier = mailboxName;
			this.CursorPosition = rowIndex;
		}

		public MigrationType MigrationType
		{
			get
			{
				return MigrationType.PublicFolder;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return MigrationUserRecipientType.Mailbox;
			}
		}

		public string Identifier { get; private set; }

		public string LocalMailboxIdentifier { get; private set; }

		public int CursorPosition { get; private set; }

		public bool SupportsRemoteIdentifier
		{
			get
			{
				return false;
			}
		}

		public string RemoteIdentifier
		{
			get
			{
				return null;
			}
		}
	}
}
